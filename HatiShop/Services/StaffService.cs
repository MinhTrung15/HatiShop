// Services/StaffService.cs
using Microsoft.AspNetCore.Hosting;
using HatiShop.Models;
using HatiShop.Repositories;
using HatiShop.Services;

namespace HatiShop.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IWebHostEnvironment _environment;

        public StaffService(IStaffRepository staffRepository, IWebHostEnvironment environment)
        {
            _staffRepository = staffRepository;
            _environment = environment;
        }

        public async Task<ServiceResult> CreateStaffAsync(Staff staff, IFormFile? avatarFile)
        {
            try
            {
                // Check unique constraints
                if (await _staffRepository.UsernameExistsAsync(staff.Username))
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập đã tồn tại" };

                if (!string.IsNullOrEmpty(staff.Email) && await _staffRepository.EmailExistsAsync(staff.Email))
                    return new ServiceResult { Success = false, Message = "Email đã tồn tại" };

                if (!string.IsNullOrEmpty(staff.PhoneNumber) && await _staffRepository.PhoneExistsAsync(staff.PhoneNumber))
                    return new ServiceResult { Success = false, Message = "Số điện thoại đã tồn tại" };

                // Handle avatar upload
                if (avatarFile != null)
                {
                    staff.AvatarPath = await SaveAvatarAsync(avatarFile);
                }

                // Generate ID if not provided
                if (string.IsNullOrEmpty(staff.Id))
                {
                    staff.Id = "STAFF_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                var result = await _staffRepository.CreateAsync(staff);
                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Tạo nhân viên thành công" : "Tạo nhân viên thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UpdateStaffAsync(Staff staff, IFormFile? avatarFile)
        {
            try
            {
                var existingStaff = await _staffRepository.GetByIdAsync(staff.Id);
                if (existingStaff == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy nhân viên" };

                // Check unique constraints excluding current staff
                if (await _staffRepository.GetByUsernameAsync(staff.Username) is Staff user && user.Id != staff.Id)
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập đã tồn tại" };

                if (!string.IsNullOrEmpty(staff.Email) &&
                    await _staffRepository.GetByEmailAsync(staff.Email) is Staff emailUser && emailUser.Id != staff.Id)
                    return new ServiceResult { Success = false, Message = "Email đã tồn tại" };

                if (!string.IsNullOrEmpty(staff.PhoneNumber) &&
                    await _staffRepository.GetByPhoneAsync(staff.PhoneNumber) is Staff phoneUser && phoneUser.Id != staff.Id)
                    return new ServiceResult { Success = false, Message = "Số điện thoại đã tồn tại" };

                // Handle avatar upload
                if (avatarFile != null)
                {
                    // Delete old avatar if exists
                    if (!string.IsNullOrEmpty(existingStaff.AvatarPath))
                    {
                        DeleteAvatar(existingStaff.AvatarPath);
                    }
                    staff.AvatarPath = await SaveAvatarAsync(avatarFile);
                }
                else
                {
                    staff.AvatarPath = existingStaff.AvatarPath;
                }

                var result = await _staffRepository.UpdateAsync(staff);
                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Cập nhật nhân viên thành công" : "Cập nhật nhân viên thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DeleteStaffAsync(string id)
        {
            try
            {
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                    return new ServiceResult { Success = false, Message = "Không tìm thấy nhân viên" };

                // Delete avatar file if exists
                if (!string.IsNullOrEmpty(staff.AvatarPath))
                {
                    DeleteAvatar(staff.AvatarPath);
                }

                var result = await _staffRepository.DeleteAsync(id);
                return new ServiceResult
                {
                    Success = result,
                    Message = result ? "Xóa nhân viên thành công" : "Xóa nhân viên thất bại"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        public async Task<Staff?> GetStaffByIdAsync(string id)
        {
            return await _staffRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _staffRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Staff>> SearchStaffAsync(string searchType, string searchValue)
        {
            return searchType?.ToLower() switch
            {
                "name" => await _staffRepository.SearchByNameAsync(searchValue),
                "id" => await _staffRepository.SearchByIdAsync(searchValue),
                "phone" => await _staffRepository.SearchByPhoneAsync(searchValue),
                "role" => await _staffRepository.SearchByRoleAsync(searchValue),
                _ => await _staffRepository.GetAllAsync()
            };
        }

        public async Task<ServiceResult> LoginAsync(string username, string password)
        {
            try
            {
                var staff = await _staffRepository.LoginAsync(username, password);
                if (staff == null)
                    return new ServiceResult { Success = false, Message = "Tên đăng nhập hoặc mật khẩu không đúng" };

                return new ServiceResult
                {
                    Success = true,
                    Message = "Đăng nhập thành công",
                    Data = staff
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
    }
}