namespace CarConsoleApp;

/// <summary>Автомобиль с ДВС и механической (ручной) коробкой.</summary>
public sealed class Toyota : MechanicalCar, IManual
{
    public Toyota()
        : base("Toyota", 5)
    {
    }

    public override string PowertrainDescription =>
        "Механическая тяга: бензиновый двигатель внутреннего сгорания.";

    public string TransmissionDescription =>
        "Механическая (ручная) коробка передач с сцеплением и выбором передач водителем.";

    public override string GetDescription()
    {
        return $"{base.GetDescription()} {PowertrainDescription} {TransmissionDescription}";
    }
}
