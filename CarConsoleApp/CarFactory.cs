namespace CarConsoleApp;

public static class CarFactory
{
    public static ICar CreateCar(CarType type)
    {
        return type switch
        {
            CarType.Tesla => new Tesla(),
            CarType.BMW => new BMW(),
            CarType.Toyota => new Toyota(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип автомобиля.")
        };
    }
}
