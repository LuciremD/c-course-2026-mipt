namespace CarConsoleApp;

public abstract class ElectricCar : ACar, IElectric
{
    protected ElectricCar(string brand, int seats)
        : base(brand, seats)
    {
    }

    public abstract string PowertrainDescription { get; }
}
