namespace payment_gateway_core.Validation.Engine
{
    public enum ErrorCode
    {
        NoAuthorized,
        IncorrectPassword,
        ValueIsNullOrEmpty,
        DataBaseError,
        UserAlreadyExists,
        InvalidValue,
        InvalidCardNumber,
        InvalidExpiryDate,
        InvalidAmount,
        InvalidCurrency,
        PaymentFailed
    }
}