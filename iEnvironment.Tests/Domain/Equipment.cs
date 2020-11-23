using System;
using iEnvironment.Domain.Models;
using Xunit;

namespace iEnvironment.Tests.Domain
{
    public class EquipmentTests
    {
        [Fact]
        public void CriarEquipamento()
        {
            //Arrange
            var equipamento = new Equipment{ Enabled = true };

            //Act
            equipamento.UpdateValue("teste", true);

            //Assert
            Assert.Equal("teste", equipamento.CurrentValue);
        }


    }
}
