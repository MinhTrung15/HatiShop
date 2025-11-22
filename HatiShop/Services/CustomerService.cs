// Services/CustomerService.cs
using Microsoft.EntityFrameworkCore;
using HatiShop.Models;
using HatiShop.Repositories;
using HatiShop.Data;

namespace HatiShop.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public CustomerService(ICustomerRepository customerRepository,
                             IWebHostEnvironment environment,
                             ApplicationDbContext context)
        {
            _customerRepository = customerRepository;
            _environment = environment;
            _context = context;
        }

        public async Task<ServiceResult> CreateCustomerAsync(Customer customer, IFormFile? avatarFile)
        {
            try
            {
                if (await _customerRepository.GetByIdAsync(customer.Id) != null)
                    return new ServiceResult { Success = false, Message = "Mã KH đã tồn tại" };

                if (await _customerRepository.GetByUsernameAsync(customer.Username) != null)
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập đã tồn tại" };

                if (!string.IsNullOrEmpty(customer.Email) &&
                    await _customerRepository.GetByEmailAsync(customer.Email) != null)
                    return new ServiceResult { Success = false, Message = "Email đã tồn tại" };

                if (!string.IsNullOrEmpty(customer.PhoneNumber) &&
                    await _customerRepository.GetByPhoneAsync(customer.PhoneNumber) != null)
                    return new ServiceResult { Success = false, Message = "Số điện thoại đã tồn tại" };

                if (avatarFile != null && avatarFile.Length > 0)
                {
                    customer.AvatarPath = await SaveAvatarAsync(avatarFile);
                }

                if (string.IsNullOrEmpty(customer.Rank))
                    customer.Rank = "ĐỒNG";

                var result = await _customerRepository.CreateAsync(customer);

                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Tạo khách hàng thành công" : "Tạo khách hàng thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateCustomerAsync(Customer customer, IFormFile? avatarFile)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id);
                if (existingCustomer == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy khách hàng" };

                var existingUsername = await _customerRepository.GetByUsernameAsync(customer.Username);
                if (existingUsername != null && existingUsername.Id != customer.Id)
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập đã tồn tại" };

                if (!string.IsNullOrEmpty(customer.Email))
                {
                    var existingEmail = await _customerRepository.GetByEmailAsync(customer.Email);
                    if (existingEmail != null && existingEmail.Id != customer.Id)
                        return new ServiceResult { Success = false, Message = "Email đã tồn tại" };
                }

                if (!string.IsNullOrEmpty(customer.PhoneNumber))
                {
                    var existingPhone = await _customerRepository.GetByPhoneAsync(customer.PhoneNumber);
                    if (existingPhone != null && existingPhone.Id != customer.Id)
                        return new ServiceResult { Success = false, Message = "Số điện thoại đã tồn tại" };
                }

                string avatarPath = existingCustomer.AvatarPath;
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingCustomer.AvatarPath))
                    {
                        DeleteAvatar(existingCustomer.AvatarPath);
                    }
                    avatarPath = await SaveAvatarAsync(avatarFile);
                }

                var customerToUpdate = new Customer
                {
                    Id = customer.Id,
                    Username = customer.Username,
                    Password = customer.Password ?? existingCustomer.Password,
                    FullName = customer.FullName,
                    Gender = customer.Gender,
                    BirthDate = customer.BirthDate,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    AvatarPath = avatarPath,
                    Revenue = customer.Revenue,
                    Rank = customer.Rank ?? existingCustomer.Rank
                };

                var result = await _customerRepository.UpdateAsync(customerToUpdate);

                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Cập nhật khách hàng thành công" : "Cập nhật khách hàng thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<string> SaveAvatarAsync(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "avatars");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(fileStream);
            }

            return $"/images/avatars/{uniqueFileName}";
        }

        private void DeleteAvatar(string avatarPath)
        {
            if (string.IsNullOrEmpty(avatarPath)) return;

            var fullPath = Path.Combine(_environment.WebRootPath, avatarPath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<ServiceResult> DeleteCustomerAsync(string id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy khách hàng" };

                if (!string.IsNullOrEmpty(customer.AvatarPath))
                {
                    DeleteAvatar(customer.AvatarPath);
                }

                var result = await _customerRepository.DeleteAsync(id);
                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Xóa khách hàng thành công" : "Xóa khách hàng thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchType, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
                return await _customerRepository.GetAllAsync();

            return searchType?.ToLower() switch
            {
                "name" => await _customerRepository.SearchByNameAsync(searchValue),
                "id" => await _customerRepository.SearchByIdAsync(searchValue),
                "phone" => await _customerRepository.SearchByPhoneAsync(searchValue),
                _ => await _customerRepository.GetAllAsync()
            };
        }
    }
}