namespace PasteboardProject.Exceptions;

public class CustomException : Exception
{
    public const string NotFoundMessage = "Карточка отсутствует в базе данных";
    public const string DefaultMessage = "Упс! Что-то пошло не так, обратитесь к администратору";
    public CustomException(string message) : base(message)
    {
    }
}