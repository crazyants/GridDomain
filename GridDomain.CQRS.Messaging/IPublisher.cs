﻿namespace GridDomain.CQRS.Messaging
{
    public interface IPublisher
    {
        void Publish<T>(T msg);
    }
}