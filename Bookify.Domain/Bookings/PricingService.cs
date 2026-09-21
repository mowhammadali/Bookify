using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;

        Money priceForPeriod = new Money(apartment.Price.Amount * period.LengthInDays, currency);

        decimal percentageUpCharge = 0;
        foreach (var amenity in apartment.Amenities)
        {
            switch (amenity)
            {
                case Amenity.GardenView or Amenity.MountainView:
                    percentageUpCharge += 0.05m;
                    break;
                case Amenity.AirConditioning:
                    percentageUpCharge += 0.01m;
                    break;
                case Amenity.Parking:
                    percentageUpCharge += 0.01m;
                    break;
                default:
                    percentageUpCharge += 0m;
                    break;
            }
        }


        Money amenityUpCharge = Money.Zero(currency);

        if (percentageUpCharge > 0)
        {
            amenityUpCharge = new Money(priceForPeriod.Amount * percentageUpCharge, currency);
        }

        Money totalPrice = Money.Zero(currency);

        totalPrice += priceForPeriod;
        totalPrice += amenityUpCharge;


        Money cleaningFee = apartment.CleaningFee;

        if (!cleaningFee.IsZero())
        {
            totalPrice += apartment.CleaningFee;
        }

        return new PricingDetails(PriceForPeriod: priceForPeriod, CleaningFee: cleaningFee,
            AmenitiesUpCharge: amenityUpCharge, TotalPrice: totalPrice);
    }
}