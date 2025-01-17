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
    public Ledger? Get(int id)
    {
        var ledger = ledgerRepository.SelectOne(id);
        return ledger;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public void Put(int id, [FromBody] Ledger ledger)
    {
        ledgerRepository.Update(ledger);
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

            try
            {
                ledgerRepository.Delete(id);
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