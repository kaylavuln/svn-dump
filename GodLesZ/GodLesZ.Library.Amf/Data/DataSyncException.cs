
using System.Collections;
using GodLesZ.Library.Amf.Data.Messages;
using GodLesZ.Library.Amf.Messaging;
using GodLesZ.Library.Amf.Messaging.Messages;

namespace GodLesZ.Library.Amf.Data {
	/// <summary>
	/// A DataSyncException describes a data conflict that arises as a result of a client attempt to commit a "sync" DataService operation. It is propagated to the client, and handled as the trigger for a conflict event.
	/// </summary>
	public class DataSyncException : MessageException {
		object _serverObject;
		IList _propertyNames;
		//DataMessage _conflictCause;

		/// <summary>
		/// Initializes a new instance of the DataSyncException class.
		/// </summary>
		/// <param name="serverVersion">Version of the Object known to be in safe, correct state.</param>
		/// <param name="propertyNames">List of properties that are in conflict.</param>
		public DataSyncException(object serverVersion, IList propertyNames) {
			_serverObject = serverVersion;
			_propertyNames = propertyNames;
		}
		/// <summary>
		/// Gets the version of the Object known to be in safe, correct state.
		/// </summary>
		public object ServerObject {
			get { return _serverObject; }
		}
		/// <summary>
		/// Gets list of properties that are in conflict.
		/// </summary>
		public IList PropertyNames {
			get { return _propertyNames; }
		}

		/*
		public DataMessage ConflictCause
		{
			get{ return _conflictCause; }
			set{ _conflictCause = value; }
		}
		*/

		internal override ErrorMessage GetErrorMessage() {
			DataErrorMessage dataErrorMessage = new DataErrorMessage(this);
			dataErrorMessage.faultCode = this.FaultCode;
			dataErrorMessage.faultString = this.Message;
			if (this.InnerException != null) {
				dataErrorMessage.faultDetail = this.InnerException.StackTrace;
				if (this.ExtendedData != null)
					this.ExtendedData["FluorineStackTrace"] = this.StackTrace;
			} else
				dataErrorMessage.faultDetail = this.StackTrace;
			dataErrorMessage.extendedData = this.ExtendedData;
			if (this.RootCause != null)
				dataErrorMessage.rootCause = this.RootCause;

			dataErrorMessage.propertyNames = _propertyNames;
			dataErrorMessage.serverObject = _serverObject;
			return dataErrorMessage;
		}
	}
}
