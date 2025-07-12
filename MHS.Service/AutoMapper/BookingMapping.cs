using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MHS.Service.DTOs;

namespace MHS.Service.AutoMapper
{
    public class BookingMapping: Profile
    {
        public BookingMapping()
        {
            CreateMap<MHS.Repository.Models.Booking, BookingResponse>();
        }
    }
}
