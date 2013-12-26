﻿using System;
using Funq;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using System.Collections.Generic;
using TDispatchConnection;

namespace ServiceStack.Hello
{

	public class Bookings
	{
		//POST create_booking attributes
		public string passenger { get; set; }
		public string customer_name { get; set; }
		public string customer_phone { get; set; }
		public string customer_email { get; set; }
		public string distance { get; set; }
		public string duration { get; set; }
		public string pickup_time { get; set; }
		public string return_time { get; set; }
		public PickupLocation pickup_location { get; set; }
		public string dropoff_location { get; set; }
		public string way_points { get; set; }
		public string extra_instructions { get; set; }
		public int luggage { get; set; }
		public int passengers { get; set; }
		public string payment_method { get; set; }
		public bool pre_paid { get; set; }
		public string incoming { get; set; }
		public string status { get; set; }

		//GET booking_list attributes
		public int limit { get; set; }
	}

	public class PickupLocation
	{
		public string address { get; set; }
		public Location location { get; set; }
		public string postcode { get; set; }
	}

	public class Location
	{
		public string lat { get; set; }
		public string lng { get; set; }
	}

	public class BookingsResponse
	{
		public string booking { get; set; }
	}

	public class BookingsService : IService
	{
		public object Get(Bookings request)
		{
			TDispatchFleetAPI tdispatch = TDispatchFleetAPI.getInstance();

			string b = "";
			if (request.limit != default(int) ) {
				b = tdispatch.bookingList (new Dictionary<string, object> {
					{ "limit", request.limit }
				});
			} else {
				b = tdispatch.bookingList (null);
			}

			return new BookingsResponse { booking = b };
		}

		public object Post(Bookings request)
		{
			try
			{
				TDispatchFleetAPI tdispatch = TDispatchFleetAPI.getInstance();

				string b = tdispatch.bookingCreate(new Dictionary<string,object>{
					{"passenger", request.passenger},
					{"customer_name", request.customer_name},
					{"customer_phone", request.customer_phone},
					{"distance", request.distance},
					{"duration", request.duration},
					{"pickup_time", request.pickup_time},
					{"pickup_location",new Dictionary<string,object>{
							{"address", request.pickup_location.address},
							{"location",new Dictionary<string,object>{
									{"lat", request.pickup_location.location.lat},
									{"lng", request.pickup_location.location.lng}
								}
							},
							{"postcode", request.pickup_location.postcode}
						}
					},
					{"dropoff_location", request.dropoff_location},
					{"way_points", request.way_points},
					{"extra_instructions", request.extra_instructions},
					{"luggage", request.luggage},
					{"passengers", request.passengers},
					{"payment_method", request.payment_method},
					{"pre_paid", request.pre_paid},
					{"status", request.status}
				});

				return new BookingsResponse { booking = b };
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				Console.WriteLine (ex.StackTrace);
				throw new ApplicationException ("Error : "+ ex.StackTrace);
			}
		}
	}


    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Create your ServiceStack web service application with a singleton AppHost.
        /// </summary>        
		public class TripThruAppHost : AppHostBase
        {
            /// <summary>
            /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
            /// </summary>
			public TripThruAppHost() : base("TripThru gateway v1", typeof(BookingsService).Assembly) {
				SetConfig(new EndpointHostConfig
					{
						GlobalResponseHeaders = {
							{ "Access-Control-Allow-Origin", "*" },
							{ "Access-Control-Allow-Methods", "GET, POST, DELETE" },
							{ "Access-Control-Allow-Headers", "Content-Type" },
						},
						DebugMode = true,
						DefaultContentType = "application/json"
					});
			}

            /// <summary>
            /// Configure the container with the necessary routes for your ServiceStack application.
            /// </summary>
            /// <param name="container">The built-in IoC used with ServiceStack.</param>
            public override void Configure(Container container)
            {
                //Register user-defined REST-ful urls. You can access the service at the url similar to the following.
                //http://localhost/ServiceStack.Hello/servicestack/hello or http://localhost/ServiceStack.Hello/servicestack/hello/John%20Doe
				//You can change /servicestack/ to a custom path in the web.config.
				Routes
					.Add<Bookings> ("/bookings");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            //Initialize your application
			(new TripThruAppHost()).Init();
        }
    }
}
