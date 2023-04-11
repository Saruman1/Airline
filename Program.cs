namespace AirLine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var flight1 = new Flight
            {
                FlightNumber = "FL123",
                PlaceOfDeparture = "New York",
                Destination = "Los Angeles",
                DepartureDate = "2023-04-15",
                ArrivalDate = "2023-04-15",
                TicketPrice = 500,
                MaxNumberOfPassengers = 100
            };

            var flight2 = new Flight
            {
                FlightNumber = "FL456",
                PlaceOfDeparture = "Chicago",
                Destination = "Miami",
                DepartureDate = "2023-04-16",
                ArrivalDate = "2023-04-16",
                TicketPrice = 300,
                MaxNumberOfPassengers = 80
            };

            var plane1 = new Plane
            {
                TypeOfPlane = "Boeing 747",
                MaxBaggageWeightPerPassenger = 50,
                MaxNumberOfPassengers = 200
            };

            var plane2 = new Plane
            {
                TypeOfPlane = "Airbus A320",
                MaxBaggageWeightPerPassenger = 40,
                MaxNumberOfPassengers = 150
            };

            var airport = new Airport(new List<Flight> { flight1, flight2 }, new List<Plane> { plane1, plane2 });

            var passenger1 = new Passenger
            {
                FirstName = "John",
                LastName = "Doe",
                PassportNumber = "123456789",
                PhoneNumber = "1234567890"
            };

            var passenger2 = new Passenger
            {
                FirstName = "Jane",
                LastName = "Doe",
                PassportNumber = "987654321",
                PhoneNumber = "0987654321"
            };

            passenger1.BookTicket(flight1, passenger1);
            passenger2.BookTicket(flight2, passenger2);

            Console.WriteLine($"Total cost of booked tickets for flight {flight1.FlightNumber}: {flight1.CalculateTotalCostOfTickets()}");
            Console.WriteLine($"Total cost of booked tickets for flight {flight2.FlightNumber}: {flight2.CalculateTotalCostOfTickets()}");

            var availableSeats = plane1.FindFreeSeats(flight1);
            Console.WriteLine($"Available seats for flight {flight1.FlightNumber}: {string.Join(", ", availableSeats)}");

            var foundFlight = airport.FindFlight("FL123");
            if (foundFlight != null)
            {
                Console.WriteLine($"Flight {foundFlight.FlightNumber} found! Departing from {foundFlight.PlaceOfDeparture} to {foundFlight.Destination} on {foundFlight.DepartureDate}");
            }



        }

        class Ticket
        {
            public string TicketNumber { get; set; }
            public Passenger Passenger { get; set; }
            public Flight Flight { get; set; }
            public int Price { get; set; }

            public bool IsBooked(List<Ticket> tickets)
            {
                foreach (var ticket in tickets)
                {
                    if (ticket.TicketNumber == TicketNumber)
                    {
                        return true;
                    }
                }
                return false;
            }

        }
        class Passenger
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            private string passportNumber;
            public string PassportNumber { get { return passportNumber; } set { passportNumber = value.Length == 9 && IsDigit(value) ? value : ""; } }
            public string PhoneNumber { get; set; }
            public string TicketNumber { get; set; }


            public void BookTicket(Flight flight, Passenger passenger)
            {
                if (flight == null || passenger == null)
                {
                    throw new ArgumentException("Flight and passenger cannot be null.");
                }

                if (flight.MaxNumberOfPassengers <= flight.registeredPassengers.Count)
                {
                    throw new ArgumentException("Flight is fully booked.");
                }

                var ticket = new Ticket
                {
                    Passenger = passenger,
                    Flight = flight,
                    Price = flight.TicketPrice,
                    TicketNumber = Guid.NewGuid().ToString()
                };

                passenger.TicketNumber = ticket.TicketNumber;

                flight.RegisterPassenger(passenger, ticket);
            }

            public void CancelTicketReservation(Flight flight, Passenger passenger)
            {
                if (flight == null || passenger == null)
                {
                    throw new ArgumentException("Flight and passenger cannot be null.");
                }

                var ticketToRemove = flight.bookedTickets.FirstOrDefault(t => t.Passenger == passenger);

                if (ticketToRemove == null)
                {
                    Console.WriteLine($"Passenger {passenger.FirstName} {passenger.LastName} does not have a reservation for flight {flight.FlightNumber}.");
                    return;
                }

                flight.bookedTickets.Remove(ticketToRemove);
                flight.registeredPassengers.Remove(passenger);
                passenger.TicketNumber = null;
                Console.WriteLine($"Reservation for passenger {passenger.FirstName} {passenger.LastName} on flight {flight.FlightNumber} has been cancelled.");
            }




            private bool IsDigit(string str)
            {
                bool state = false;
                foreach (char c in str)
                {
                    if (char.IsDigit(c))
                        state = true;
                    else
                    {
                        return false;
                    }
                }
                return state;
            }

        }
        class Flight
        {
            public string FlightNumber { get; set; }
            public string PlaceOfDeparture { get; set; }
            public string Destination { get; set; }
            public string DepartureDate { get; set; }
            public string ArrivalDate { get; set; }
            public int TicketPrice { get; set; }
            public int MaxNumberOfPassengers { get; set; }
            public List<Passenger> registeredPassengers { get; set; }
            public List<Ticket> bookedTickets { get; set; }

            public Flight()
            {
                registeredPassengers = new List<Passenger>();
                bookedTickets = new List<Ticket>();
            }

            public void RegisterPassenger(Passenger passenger, Ticket ticket)
            {
                if (registeredPassengers.Count < MaxNumberOfPassengers)
                {
                    registeredPassengers.Add(passenger);
                    bookedTickets.Add(ticket);
                    passenger.TicketNumber = ticket.TicketNumber;
                    Console.WriteLine($"Passenger {passenger.FirstName} {passenger.LastName} has been registered for the flight {FlightNumber}");
                }
                else
                {
                    Console.WriteLine($"The flight {FlightNumber} is already full, cannot register passenger {passenger.FirstName} {passenger.LastName}");
                }
            }
            public double CalculateTotalCostOfTickets()
            {
                double totalCost = 0;
                foreach (Ticket ticket in bookedTickets)
                {
                    totalCost += ticket.Price;
                }
                return totalCost;
            }

        }
        class Plane
        {
            public string TypeOfPlane { get; set; }
            public int MaxBaggageWeightPerPassenger { get; set; }
            public int MaxNumberOfPassengers { get; set; }

            public List<int> FindFreeSeats(Flight flight)
            {
                List<int> availableSeats = new List<int>();
                for (int i = flight.registeredPassengers.Count + 1; i <= MaxNumberOfPassengers; i++)
                {
                    if (!flight.bookedTickets.Any(ticket => ticket.Passenger.TicketNumber == i.ToString()))
                    {
                        availableSeats.Add(i);
                    }
                }
                return availableSeats;
            }

        }
        class Airport
        {
            public string Location { get; set; }
            public List<Flight> flights { get; set; }
            public List<Plane> planes { get; set; }
            public Airport(List<Flight> flights, List<Plane> planes)
            {
                this.flights = flights;
                this.planes = planes;
            }

            public Flight? FindFlight(string flightNumber)
            {
                Flight? flight = flights.Find(flight => flight.FlightNumber == flightNumber);
                if (flight == null)
                {
                    Console.WriteLine("No flights were found under this number.");
                    return null;
                }
                else
                    return flight;
            }
        }
        class AirlineManagementSystem
        {
            public List<Passenger> passengers { get; set; }
            public List<Airport> airports { get; set; }
            public List<Ticket> bookedTickets { get; set; }
            public List<Flight> flights { get; set; }
            public AirlineManagementSystem()
            {
                passengers = new List<Passenger>();

                airports = new List<Airport>();

                bookedTickets = new List<Ticket>();

                flights = new List<Flight>();


            }

            public Passenger? FindPassenger(string passportNumber)
            {
                var passenger = passengers.Find(passenger => passenger.PassportNumber == passportNumber);

                if (passenger == null)
                {
                    Console.WriteLine("No passengers were found under such passport number.");
                    return null;
                }
                else
                    return passenger;



            }
            public Ticket? FindBookedTicked(string ticketNumber)
            {
                var ticket = bookedTickets.Find(ticket => ticket.TicketNumber == ticketNumber);

                if (ticket == null)
                {
                    Console.WriteLine("No passengers were found under such passport number.");
                    return null;
                }
                else
                    return ticket;
            }
            public Flight? FindFlightByDate(string departureDate)
            {
                var flight = flights.FirstOrDefault(flight => flight.DepartureDate == departureDate);

                if (flight == null)
                {
                    Console.WriteLine("No flights were found for the given date and airport.");
                    return null;
                }
                else
                {
                    return flight;
                }
            }
            public void AddFlight(Flight flight)
            {
                flights.Add(flight);
                Console.WriteLine("Flight added.");
            }
            public void AddAirport(Airport airport)
            {
                airports.Add(airport);
                Console.WriteLine("Airport added.");
            }
            public void AddPlane(Plane plane, Airport airport)
            {
                airport.planes.Add(plane);
                Console.WriteLine("Plane added.");
            }
        }
    }
}