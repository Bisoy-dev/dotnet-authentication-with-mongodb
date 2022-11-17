namespace LearnHttpContext.Dtos;

public record UserDto(string Email, string Password);

public record CreateUserDto(string Email, string Password, string[] Roles);

public record CreateRole(string[] Names);