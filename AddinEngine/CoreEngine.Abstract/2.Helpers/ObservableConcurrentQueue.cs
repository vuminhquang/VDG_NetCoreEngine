using System;
using System.Collections.Concurrent;

namespace AddinEngine.Abstract
{
    public class ObservableConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public enum NotifyConcurrentQueueChangedAction
        {
            /// <summary>
            ///     The enqueue
            /// </summary>
            Enqueue, 

            /// <summary>
            ///     The de-queue
            /// </summary>
            Dequeue, 

            /// <summary>
            ///     The peek
            /// </summary>
            Peek, 

            /// <summary>
            ///     The empty
            /// </summary>
            Empty
        }

        /// <summary>
        /// The notify concurrent queue changed event args.
        /// </summary>
        /// <typeparam name="T1">
        /// The item type
        /// </typeparam>
        public class NotifyConcurrentQueueChangedEventArgs<T1> : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="NotifyConcurrentQueueChangedEventArgs{T}"/> class.
            /// </summary>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <param name="changedItem">
            /// The changed item.
            /// </param>
            public NotifyConcurrentQueueChangedEventArgs(NotifyConcurrentQueueChangedAction action, T1 changedItem)
            {
                this.Action      = action;
                this.ChangedItem = changedItem;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="NotifyConcurrentQueueChangedEventArgs{T}"/> class.
            /// </summary>
            /// <param name="action">
            /// The action.
            /// </param>
            public NotifyConcurrentQueueChangedEventArgs(NotifyConcurrentQueueChangedAction action)
            {
                this.Action = action;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the action.
            /// </summary>
            /// <value>
            ///     The action.
            /// </value>
            public NotifyConcurrentQueueChangedAction Action { get; private set; }

            /// <summary>
            ///     Gets the changed item.
            /// </summary>
            /// <value>
            ///     The changed item.
            /// </value>
            public T1 ChangedItem { get; private set; }

            #endregion
        }
        
        public delegate void ConcurrentQueueChangedEventHandler<T1>(
            object sender, 
            NotifyConcurrentQueueChangedEventArgs<T1> args);

        #region Public Events

        /// <summary>
        ///     Occurs when concurrent queue elements [changed].
        /// </summary>
        public event ConcurrentQueueChangedEventHandler<T> ContentChanged;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the end of the <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/>
        ///     . The value can be a null reference (Nothing in Visual Basic) for reference types.
        /// </param>
        public new void Enqueue(T item)
        {
            base.Enqueue(item);

            // Raise event added event
            this.OnContentChanged(
                new NotifyConcurrentQueueChangedEventArgs<T>(NotifyConcurrentQueueChangedAction.Enqueue, item));
        }

        /// <summary>
        /// Attempts to remove and return the object at the beginning of the
        ///     <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/>.
        /// </summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, <paramref name="result"/> contains the
        ///     object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the beginning of the
        ///     <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/> successfully; otherwise, false.
        /// </returns>
        public new bool TryDequeue(out T result)
        {
            if (!base.TryDequeue(out result))
            {
                return false;
            }

            // Raise item dequeued event
            this.OnContentChanged(
                new NotifyConcurrentQueueChangedEventArgs<T>(NotifyConcurrentQueueChangedAction.Dequeue, result));

            if (this.IsEmpty)
            {
                // Raise Queue empty event
                this.OnContentChanged(
                    new NotifyConcurrentQueueChangedEventArgs<T>(NotifyConcurrentQueueChangedAction.Empty));
            }

            return true;
        }

        /// <summary>
        /// Attempts to return an object from the beginning of the
        ///     <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/> without removing it.
        /// </summary>
        /// <param name="result">
        /// When this method returns, <paramref name="result"/> contains an object from the beginning of the
        ///     <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1"/> or an unspecified value if the operation failed.
        /// </param>
        /// <returns>
        /// true if and object was returned successfully; otherwise, false.
        /// </returns>
        public new bool TryPeek(out T result)
        {
            var retValue = base.TryPeek(out result);
            if (retValue)
            {
                // Raise item dequeued event
                this.OnContentChanged(
                    new NotifyConcurrentQueueChangedEventArgs<T>(NotifyConcurrentQueueChangedAction.Peek, result));
            }

            return retValue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="args">
        /// The <see cref="NotifyConcurrentQueueChangedEventArgs{T}"/> instance containing the event data.
        /// </param>
        private void OnContentChanged(NotifyConcurrentQueueChangedEventArgs<T> args)
        {
            var handler = this.ContentChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion
    }
}
