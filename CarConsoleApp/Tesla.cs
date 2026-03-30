namespace CarConsoleApp;

/// <summary>Электромобиль с автоматической (условно одноступенчатой) трансмиссией.</summary>
public sealed class Tesla : ElectricCar, IAutomatic
{
    public Tesla()
        : base("Tesla", 5)
    {
    }

    public override string PowertrainDescription =>
        "Электрический привод: высоковольтная батарея и тяговые электромоторы.";

    public string TransmissionDescription =>
        "Автоматическая трансмиссия в виде редуктора с фиксированным передаточным числом.";

    public override string GetDescription()
    {
        return $"{base.GetDescription()} {PowertrainDescription} {TransmissionDescription}";
    }
}
