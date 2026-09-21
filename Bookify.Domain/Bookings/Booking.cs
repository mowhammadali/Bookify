using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public class Booking : Entity
{
    private Booking(Guid id, Guid apartmentId, Guid userId, DateRange duration, Money priceForPeriod, Money cleaningFee,
        Money amenitiesUpCharge, Money totalPrice, BookingStatus status, DateTime createOnUtc) : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaningFee = cleaningFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreateOnUtc = createOnUtc;
    }

    public Guid ApartmentId { get; private set; }
    public Guid UserId { get; private set; }
    public DateRange Duration { get; private set; }
    public Money PriceForPeriod { get; private set; }
    public Money CleaningFee { get; private set; }
    public Money AmenitiesUpCharge { get; private set; }
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreateOnUtc { get; private set; }
    public DateTime? ConfirmedOnUtc { get; private set; }
    public DateTime? RejectedOnUtc { get; private set; }
    public DateTime? CompletedOnUtc { get; private set; }
    public DateTime? CancelledOnUtc { get; private set; }

    public Booking Reserve(Apartment apartment, Guid userId, DateRange duration, DateTime utcTime,
        PricingService pricingService)
    {
        PricingDetails pricingDetails = pricingService.CalculatePrice(apartment, duration);

        Booking booking = new Booking(Guid.NewGuid(), apartmentId: apartment.Id, userId, duration,
            priceForPeriod: pricingDetails.PriceForPeriod, cleaningFee: pricingDetails.CleaningFee,
            amenitiesUpCharge: pricingDetails.AmenitiesUpCharge, totalPrice: pricingDetails.TotalPrice,
            status: BookingStatus.Reserved, DateTime.UtcNow);

        apartment.LastBookedOnUtc = DateTime.UtcNow;

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        return booking;
    }
}