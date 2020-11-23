using System;
namespace iEnvironment.Domain.Exceptions
{

    [Serializable]
    public class EquipmentOfflineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        public EquipmentOfflineException() : base("Equipment is offline!")
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public EquipmentOfflineException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public EquipmentOfflineException(string message, System.Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EquipmentOfflineException"/> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected EquipmentOfflineException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}
