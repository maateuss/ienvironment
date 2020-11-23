﻿using System;
namespace iEnvironment.Domain.Exceptions
{

    [Serializable]
    public class EquipmentMisconfiguratedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        public EquipmentMisconfiguratedException() : base("Equipment is misconfigurated, please check!")
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public EquipmentMisconfiguratedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public EquipmentMisconfiguratedException(string message, System.Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected EquipmentMisconfiguratedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
