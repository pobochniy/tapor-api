using System.Data;
using Tapor.Shared.Dtos.Auth;
using Tapor.Shared.Enums;

namespace Tapor.Services;

public class AuthService
{
        public void Register(RegisterDto dto)
        {
            // проверим поля на уникальность в базе

            // создатим хеш пароля
            
            // добавим пользователя в базу
        }

        public UserDto LogIn(LoginDto dto)
        {
            // найдем пользователя в базе

            // сверим хеш пароля в базе с хешом из модели
            if (dto.Login != "Shalopai")
                throw new UnauthorizedAccessException();

            var res = new UserDto
            {
                Id = Guid.NewGuid(),
                UserName = dto.Login,
                Email = "admin@tapor.ru",
                Phone = null,
                Roles = new List<RoleEnum>()
                {
                    RoleEnum.RoleManagement,
                    RoleEnum.IssueCrud
                }
            };

            return res;
        }
}