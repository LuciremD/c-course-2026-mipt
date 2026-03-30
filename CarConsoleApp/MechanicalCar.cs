namespace CarConsoleApp;

public abstract class MechanicalCar : ACar, IMechanical
{
    protected MechanicalCar(string brand, int seats)
        : base(brand, seats)
    {
    }

    public abstract string PowertrainDescription { get; }
}
