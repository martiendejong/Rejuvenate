using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rejuvenate
{
    public interface IChangePublishingDbContext : IDbContextWithCustomDbSet, IDbContextWithSaveEvent
    {
        /// <summary>
        /// The ChangePublisher for all entities.
        /// Also provides ChangePublisher for the entity types that are defined on the DbContext.
        /// </summary>
        //IChangesPublisherFactory AllChangesPublisher { get; }

        /// <summary>
        /// Calls the ChangePublisher to get the ChangePublisher for an entity type.
        /// </summary>
        /// <typeparam name="EntityType">An entity type that is defined on the DbContext.</typeparam>
        /// <returns>ChangePublisher for that entity type</returns>
        IChangesPublisher<EntityType> ChangesPublisher<EntityType>() where EntityType : class, new();
    }
}
