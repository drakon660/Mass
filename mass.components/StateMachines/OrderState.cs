﻿using Automatonymous;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace mass.components.StateMachines
{
    public class OrderState : SagaStateMachineInstance, IVersionedSaga
    {
        [BsonId]
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? SubmitDate { get; set; }
    }

}
