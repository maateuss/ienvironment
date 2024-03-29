﻿using System;
using System.Threading;
using iEnvironment.Domain.Exceptions;
using iEnvironment.Domain.Models;
using Xunit;

namespace iEnvironment.Tests.Domain
{
    public class MockedEqp : Equipment
    {
        public override bool ValidateNew()
        {
            throw new NotImplementedException();
        }

        public override Equipment ValidateUpdate()
        {
            throw new NotImplementedException();
        }
    }

    public class EquipmentTests
    {
        [Fact]
        public void CriarEquipamento()
        {
            //Arrange
            var equipamento = new MockedEqp{ Enabled = true };

            //Act
            equipamento.UpdateValue("teste", true);

            //Assert
            Assert.Equal("teste", equipamento.CurrentValue);
        }

        [Fact]
        public void EquipamentoDeveMudarDeEstadoAposAutoDisconnect()
        {
            //Arrange
            var equipamento = new MockedEqp { Enabled = true, AutoDisconnectSeconds = 1 };

            equipamento.UpdateValue("teste", true);

            //Assert Arrange is valid
            Assert.True(equipamento.Alive);

            //Act
            Thread.Sleep(2000);

            //Assert
            Assert.False(equipamento.Alive);
        }

        [Fact]
        public void EquipamentoNaoPodeAceitarAutoDisconnectNegativo()
        {
            //Arrange
            var equipamento = new MockedEqp();

            //Assert

            Assert.Throws<EquipmentMisconfiguratedException>(() =>
            {
                //Act
                equipamento.AutoDisconnectSeconds = -60;
            });

        }
    }
}
