// Services/CustomerService.cs
using Microsoft.Data.SqlClient;
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
                // Check unique constraints using existing procedures
                var existingUsernames = await ExecuteStoredProcedureAsync("LoadUsername");
                if (existingUsernames.Any(u => u.ToString() == customer.Username))
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập đã tồn tại" };

                var existingEmails = await ExecuteStoredProcedureAsync("LoadEmail");
                if (!string.IsNullOrEmpty(customer.Email) && existingEmails.Any(e => e.ToString() == customer.Email))
                    return new ServiceResult { Success = false, Message = "Email đã tồn tại" };

                var existingPhones = await ExecuteStoredProcedureAsync("LoadPhone");
                if (!string.IsNullOrEmpty(customer.PhoneNumber) && existingPhones.Any(p => p.ToString() == customer.PhoneNumber))
                    return new ServiceResult { Success = false, Message = "Số điện thoại đã tồn tại" };

                // Handle avatar upload
                if (avatarFile != null)
                {
                    customer.AvatarPath = await SaveAvatarAsync(avatarFile);
                }

                // Generate ID if not provided
                if (string.IsNullOrEmpty(customer.Id))
                {
                    customer.Id = "CUS_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // Execute stored procedure for creation
                var result = await ExecuteCreateCustomerProcedureAsync(customer);

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

        private async Task<List<object>> ExecuteStoredProcedureAsync(string procedureName)
        {
            var result = new List<object>();
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(reader[0]);
                        }
                    }
                }
            }
            return result;
        }

        private async Task<bool> ExecuteCreateCustomerProcedureAsync(Customer customer)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC CreateNewCustomer @Id, @Username, @Password, @FullName, @Gender, @BirthDate, @PhoneNumber, @Email, @Address, @AvatarPath, @Revenue, @Rank",
                    new SqlParameter("@Id", customer.Id),
                    new SqlParameter("@Username", customer.Username),
                    new SqlParameter("@Password", customer.Password),
                    new SqlParameter("@FullName", customer.FullName),
                    new SqlParameter("@Gender", (object)customer.Gender ?? DBNull.Value),
                    new SqlParameter("@BirthDate", (object)customer.BirthDate ?? DBNull.Value),
                    new SqlParameter("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value),
                    new SqlParameter("@Email", (object)customer.Email ?? DBNull.Value),
                    new SqlParameter("@Address", (object)customer.Address ?? DBNull.Value),
                    new SqlParameter("@AvatarPath", (object)customer.AvatarPath ?? DBNull.Value),
                    new SqlParameter("@Revenue", customer.Revenue),
                    new SqlParameter("@Rank", customer.Rank)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Các phương thức khác giữ nguyên...
        public async Task<ServiceResult> UpdateCustomerAsync(Customer customer, IFormFile? avatarFile)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id);
                if (existingCustomer == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy khách hàng" };

                // Handle avatar upload
                if (avatarFile != null)
                {
                    if (!string.IsNullOrEmpty(existingCustomer.AvatarPath))
                    {
                        DeleteAvatar(existingCustomer.AvatarPath);
                    }
                    customer.AvatarPath = await SaveAvatarAsync(avatarFile);
                }
                else
                {
                    customer.AvatarPath = existingCustomer.AvatarPath;
                }

                // Execute stored procedure for update
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC EditCustomerInfo @Id, @FullName, @Gender, @BirthDate, @PhoneNumber, @Email, @Address, @AvatarPath, @Revenue, @Rank",
                    new SqlParameter("@Id", customer.Id),
                    new SqlParameter("@FullName", customer.FullName),
                    new SqlParameter("@Gender", (object)customer.Gender ?? DBNull.Value),
                    new SqlParameter("@BirthDate", (object)customer.BirthDate ?? DBNull.Value),
                    new SqlParameter("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value),
                    new SqlParameter("@Email", (object)customer.Email ?? DBNull.Value),
                    new SqlParameter("@Address", (object)customer.Address ?? DBNull.Value),
                    new SqlParameter("@AvatarPath", (object)customer.AvatarPath ?? DBNull.Value),
                    new SqlParameter("@Revenue", customer.Revenue),
                    new SqlParameter("@Rank", customer.Rank)
                );

                return new ServiceResult
                {
                    Success = true,
                    Message = "Cập nhật khách hàng thành công"
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

        // Các phương thức khác giữ nguyên từ trước...
        public async Task<ServiceResult> DeleteCustomerAsync(string id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy khách hàng" };

                // Delete avatar file if exists
                if (!string.IsNullOrEmpty(customer.AvatarPath))
                {
                    DeleteAvatar(customer.AvatarPath);
                }

                // Use repository to delete
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