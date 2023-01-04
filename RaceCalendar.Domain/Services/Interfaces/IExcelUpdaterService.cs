using RaceCalendar.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IExcelUpdaterService
{
    Task Update(Race race);
}
