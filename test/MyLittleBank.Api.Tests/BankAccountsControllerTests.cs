using Microsoft.AspNetCore.Mvc;
using Moq;
using MyLittleBank.Controllers;
using MyLittleBank.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Threenine.Data;
using Threenine.Data.Paging;
using Xunit;

namespace MyLittleBank.Api.Tests
{
    public class BankAccountsControllerTests
    {
        private readonly BankAccountsController _bankAccountsController;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IRepository<BankAccount>> _bankAccountRepositoryMock = new Mock<IRepository<BankAccount>>();
        private readonly Mock<IRepositoryReadOnly<BankAccount>> _bankAccountReadOnlyRepositoryMock = new Mock<IRepositoryReadOnly<BankAccount>>();

        public BankAccountsControllerTests()
        {
            _bankAccountsController = new BankAccountsController(_unitOfWorkMock.Object);
        }

        #region GetAll

        [Fact]
        public void GetAll_Tests()
        {
            // Arrange
            var expectedStatusCode = 200;
            _unitOfWorkMock
                .Setup(uow => uow.GetReadOnlyRepository<BankAccount>())
                .Returns(_bankAccountReadOnlyRepositoryMock.Object);

            var bankAccounts = new BankAccount[] {
                    new BankAccount
                    {
                        Id = 1,
                        Balance = 102.45M,
                        Number = "PBO1221323",
                        IsLocked = false
                    },
                    new BankAccount
                    {
                        Id = 2,
                        Balance = 10002.98M,
                        Number = "PIC9984567",
                        IsLocked = false
                    }
            };
            _bankAccountReadOnlyRepositoryMock
                .Setup(ror => ror.GetList<BankAccount>(null, null, null, null, 0, 20, true))
                .Returns(bankAccounts.ToPaginate(0, 20));

            // Act
            var result = _bankAccountsController.Get();
            var or = (result as ActionResult<IList<BankAccount>>)?.Result as ObjectResult;

            // Assert
            Assert.NotNull(or);
            Assert.Equal(expectedStatusCode, or.StatusCode);
            Assert.NotEmpty(or.Value as IList<BankAccount>);
            Assert.Equal(bankAccounts.Length, (or.Value as IList<BankAccount>).Count);
        }

        #endregion

        #region Get

        [Theory]
        [InlineData(200, 1)]
        [InlineData(404, 2)]
        public void Get_Tests(int? expectedStatusCode, int bankAccountId)
        {
            // Arrange
            _unitOfWorkMock
                .Setup(uow => uow.GetReadOnlyRepository<BankAccount>())
                .Returns(_bankAccountReadOnlyRepositoryMock.Object);

            var bankAccount = expectedStatusCode == 200
                ? new BankAccount
                {
                    Id = bankAccountId,
                    Balance = 102.45M,
                    Number = "PBO1221323",
                    IsLocked = false
                }
                : null;
            _bankAccountReadOnlyRepositoryMock
                .Setup(ror => ror.Single(It.IsNotNull<Expression<Func<BankAccount, bool>>>(), null, null, true))
                .Returns(bankAccount);

            // Act
            var result = _bankAccountsController.Get(bankAccountId);


            // Assert
            if ((result as ActionResult<BankAccount>)?.Result is ObjectResult)
            {
                var or = (result as ActionResult<BankAccount>)?.Result as ObjectResult;

                Assert.NotNull(or);
                Assert.Equal(expectedStatusCode, or.StatusCode);
                if (expectedStatusCode == 200)
                {
                    Assert.NotNull(or.Value as BankAccount);
                    Assert.Equal(bankAccount.Id, (or.Value as BankAccount).Id);
                }
            }
            else
            {
                var scr = (result as ActionResult<BankAccount>)?.Result as StatusCodeResult;

                Assert.NotNull(scr);
                Assert.Equal(expectedStatusCode, scr.StatusCode);
            }
        }

        #endregion

        #region Post

        [Fact]
        public void Post_Successfully_Tests()
        {
            // Arrange
            var expectedStatusCode = 201;
            _unitOfWorkMock
                .Setup(uow => uow.GetRepository<BankAccount>())
                .Returns(_bankAccountRepositoryMock.Object);

            var bankAccount = new BankAccount
            {
                Id = 0,
                Balance = 102.45M,
                Number = "PBO1221323",
                IsLocked = false
            };
            _bankAccountRepositoryMock
                .Setup(ror => ror.Single(It.IsNotNull<Expression<Func<BankAccount, bool>>>(), null, null, true))
                .Returns((BankAccount)null);

            // Act
            var result = _bankAccountsController.Post(bankAccount);
            var or = (result as ActionResult<BankAccount>)?.Result as ObjectResult;

            // Assert
            Assert.NotNull(or);
            Assert.Equal(expectedStatusCode, or.StatusCode);
            Assert.NotNull(or.Value as BankAccount);
        }

        [Fact]
        public void Post_Bank_Account_Already_Exist_Tests()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(uow => uow.GetRepository<BankAccount>())
                .Returns(_bankAccountRepositoryMock.Object);

            var bankAccount = new BankAccount
            {
                Id = 0,
                Balance = 102.45M,
                Number = "PBO1221323",
                IsLocked = false
            };
            _bankAccountRepositoryMock
                .Setup(ror => ror.Single(It.IsNotNull<Expression<Func<BankAccount, bool>>>(), null, null, true))
                .Returns(bankAccount);

            // Act
            try
            {
                var result = _bankAccountsController.Post(bankAccount);
            }
            catch (InvalidOperationException ex)
            {
                // Assert
                Assert.Contains($"and number: {bankAccount.Number} already exists", ex.Message);
            }
        }

        #endregion
    }
}
