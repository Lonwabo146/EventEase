using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings - consolidated view with search and filters
        public async Task<IActionResult> Index(
            string searchString,
            string eventTypeFilter,
            DateTime? startDate,
            DateTime? endDate,
            string availabilityFilter)
        {
            // Preserve filter values for the view
            ViewData["CurrentFilter"] = searchString;
            ViewData["EventTypeFilter"] = eventTypeFilter;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["AvailabilityFilter"] = availabilityFilter;

            // Populate EventType dropdown
            ViewData["EventTypes"] = new SelectList(
                _context.EventType.OrderBy(e => e.EventTypeName),
                "EventTypeName",
                "EventTypeName"
            );

            var bookings = _context.BookingDetailsView.AsQueryable();

            // Search by BookingId or Event Name
            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumber = int.TryParse(searchString, out int bookingId);
                bookings = bookings.Where(b =>
                    (isNumber && b.BookingId == bookingId) ||
                    b.EventName.Contains(searchString));
            }

            // Filter by Event Type
            if (!string.IsNullOrEmpty(eventTypeFilter))
            {
                bookings = bookings.Where(b => b.EventTypeName == eventTypeFilter);
            }

            // Filter by Date Range
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.EventDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.EventDate <= endDate.Value);
            }

            // Filter by Venue Availability
            if (!string.IsNullOrEmpty(availabilityFilter))
            {
                bool isAvailable = availabilityFilter == "available";
                bookings = bookings.Where(b => b.IsAvailable == isAvailable);
            }

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(
                _context.Venues.Where(v => v.IsAvailable),
                "VenueId",
                "VenueName"
            );
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            ModelState.Remove("Event");
            ModelState.Remove("Venue");

            if (ModelState.IsValid)
            {
                // Check for double booking (same venue, same date)
                bool conflict = await _context.Bookings.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == booking.BookingDate.Date);

                if (conflict)
                {
                    TempData["Error"] = "This venue is already booked for the selected date.";
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(
                        _context.Venues.Where(v => v.IsAvailable),
                        "VenueId", "VenueName", booking.VenueId);
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(
                _context.Venues.Where(v => v.IsAvailable),
                "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            ModelState.Remove("Event");
            ModelState.Remove("Venue");

            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                bool conflict = await _context.Bookings.AnyAsync(b =>
                    b.BookingId != booking.BookingId &&
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == booking.BookingDate.Date);

                if (conflict)
                {
                    TempData["Error"] = "This venue is already booked for the selected date.";
                    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}