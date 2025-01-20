using Bank.Core.Models;
using Bank.Web.Dto;
using Bank.DbAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bank.Web.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LedgersController(ILedgerRepository ledgerRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrators,Users")]
    public IEnumerable<Ledger> Get()
    {
        var allLedgers = ledgerRepository.GetAllLedgers();
        return allLedgers;
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrators,Users")]
    public IActionResult Get(int id)
    {
        var ledger = ledgerRepository.SelectOne(id);
        if (ledger == null) {
            return NotFound();
        }
        return Ok(ledger);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Put(int id, [FromBody] Ledger ledger)
    {
        var _ledger = ledgerRepository.SelectOne(id);
        if (_ledger == null) {
            return NotFound();
        }
        ledgerRepository.Update(ledger);
        return Ok();
    }


    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Post([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LedgerDto ledger)
    {

        return await Task.Run(() =>
        {
            IActionResult response = Ok();

            try
            {
                ledgerRepository.Create(ledger.name);
            }
            catch (ConstraintException ce)
            {
                return BadRequest(ce.Message);
            }
            catch (Exception)
            {
                response = Conflict();
            }

            return response;
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> Delete(int id)
    {

        return await Task.Run(() =>
        {
            IActionResult response = Ok();

            var ledger = ledgerRepository.SelectOne(id);
            if (ledger == null) {
                return NotFound();
            }

            try
            {
                ledgerRepository.Delete(ledger);
            }
            catch (ConstraintException ce)
            {
                return BadRequest(ce.Message);
            }
            catch (Exception)
            {
                response = Conflict();
            }

            return response;
        });
    }

}