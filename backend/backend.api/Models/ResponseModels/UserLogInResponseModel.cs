namespace backend.api.Models.ResponseModels
{
    public class UserLogInResponseModel
    {
        public string Token { get; set; }
        public string Message { get; set; } = "Login exitoso";
    }
}
