namespace IdentityService.WebAPI.Events;

public record SendEmailVerifiedCodeEvent(string ToEmail, string VerifyCode);
