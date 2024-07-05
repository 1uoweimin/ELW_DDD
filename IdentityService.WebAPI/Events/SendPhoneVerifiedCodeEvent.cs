namespace IdentityService.WebAPI.Events;

public record SendPhoneVerifiedCodeEvent(string ToPhoneNumber, string VerifyCode);
