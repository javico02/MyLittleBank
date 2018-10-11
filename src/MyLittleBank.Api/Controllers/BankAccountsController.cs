using Microsoft.AspNetCore.Mvc;
using MyLittleBank.Api.Properties;
using MyLittleBank.Entities;
using System;
using System.Collections.Generic;
using Threenine.Data;

namespace MyLittleBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankAccountsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IList<BankAccount>> Get()
        {
            // Getting repository 
            var repo = _unitOfWork.GetReadOnlyRepository<BankAccount>();
            return Ok(repo.GetList<BankAccount>(null, null, null, null, 0, 20, true).Items);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<BankAccount> Get([FromRoute]int id)
        {
            // Getting repository 
            var repo = _unitOfWork.GetReadOnlyRepository<BankAccount>();

            // Getting bankAccount with matched id
            var bankAccount = repo.Single(ba => ba.Id == id, null, null, true);

            // If not exist a bank account with the specified id
            if (bankAccount == null)
                return NotFound();

            return Ok(bankAccount);
        }

        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<BankAccount> Post([FromBody] BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Getting repository 
            var repo = _unitOfWork.GetRepository<BankAccount>();

            // Getting bankAccount with matched number
            var persistedBankAccount = repo.Single(ba => ba.Number == bankAccount.Number, null, null, true);

            // If exists a bank account with the specific number
            if (persistedBankAccount != null)
                throw new InvalidOperationException(string.Format(Resources.exception_BankAccountAlreadyExist, persistedBankAccount.Id, persistedBankAccount.Number));

            // Persist bankAccount
            repo.Add(bankAccount);
            _unitOfWork.SaveChanges();

            return Created($"bankAccounts/{bankAccount.Id}", bankAccount);
        }
    }
}
