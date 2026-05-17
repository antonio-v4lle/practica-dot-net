namespace UltraPlatform.Worker.Interfaces;

public interface IDataValidationHelper
{
    Task<bool> SecondValidation();
}