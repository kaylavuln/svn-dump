using System;

using GodLesZ.Library.Amf.Messaging.Api.Event;
using GodLesZ.Library.Amf.Messaging.Api.Service;

namespace GodLesZ.Library.Amf.Messaging.Api {
	/// <summary>
	/// The scope handler controls actions performed against a scope object, and also is notified of all events.
	/// 
	/// Gives fine grained control over what actions can be performed with the can*
	/// methods. Allows for detailed reporting on what is happening within the scope
	/// with the on* methods. This is the core interface users implement to create
	/// applications.
	/// </summary>
	[CLSCompliant(false)]
	public interface IScopeHandler : IEventHandler {
		/// <summary>
		/// Called when a scope is created for the first time.
		/// </summary>
		/// <param name="scope">The new scope object.</param>
		/// <returns><code>true</code> to allow, <code>false</code> to deny.</returns>
		bool Start(IScope scope);
		/// <summary>
		/// Called just before a scope is disposed.
		/// </summary>
		/// <param name="scope"></param>
		void Stop(IScope scope);
		/// <summary>
		/// Called just before every connection to a scope. You can pass additional
		/// parameters from client using <code>NetConnection.connect</code> method (see
		/// below).
		/// </summary>
		/// <param name="connection">Connection object.</param>
		/// <param name="scope"></param>
		/// <param name="parameters">List of params passed from client via <code>NetConnection.connect</code> method. All parameters but the first one passed to <code>NetConnection.connect</code> method are available as parameters array.</param>
		/// <returns><code>true</code> to allow, <code>false</code> to deny.</returns>
		bool Connect(IConnection connection, IScope scope, object[] parameters);
		/// <summary>
		/// Called just after the a connection is disconnected.
		/// </summary>
		/// <param name="connection">Connection object.</param>
		/// <param name="scope">Scope object.</param>
		void Disconnect(IConnection connection, IScope scope);
		/// <summary>
		/// Called just before a child scope is added.
		/// </summary>
		/// <param name="scope">Scope that will be added.</param>
		/// <returns></returns>
		bool AddChildScope(IBasicScope scope);
		/// <summary>
		/// Called just after a child scope has been removed.
		/// </summary>
		/// <param name="scope">Scope that has been removed.</param>
		void RemoveChildScope(IBasicScope scope);
		/// <summary>
		/// Called just before a client enters the scope.
		/// </summary>
		/// <param name="client">Client object.</param>
		/// <param name="scope"></param>
		/// <returns><code>true</code> to allow, <code>false</code> to deny connection.</returns>
		bool Join(IClient client, IScope scope);
		/// <summary>
		/// Called just after the client leaves the scope.
		/// </summary>
		/// <param name="client">Client object.</param>
		/// <param name="scope">Scope object.</param>
		void Leave(IClient client, IScope scope);
		/// <summary>
		/// Called when a service is called.
		/// </summary>
		/// <param name="connection">Connection object.</param>
		/// <param name="call">Call object.</param>
		/// <returns><code>true</code> to allow, <code>false</code> to deny.</returns>
		bool ServiceCall(IConnection connection, IServiceCall call);
	}
}
