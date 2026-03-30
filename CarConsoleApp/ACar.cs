namespace CarConsoleApp;

public abstract class ACar : ICar
{
    public string Brand { get; }
    public int Seats { get; }

    protected ACar(string brand, int seats)
    {
        Brand = brand;
        Seats = seats;
    }

    /// <summary>Базовое описание: марка и количество мест.</summary>
    public virtual string GetDescription()
    {
        return $"Автомобиль «{Brand}», посадочных мест: {Seats}.";
    }
}
