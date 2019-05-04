﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangePublishingDbContext
{
    public class ChangedRelationship
    {
        public ChangedRelationship(EntityState state, object parent, object child)
        {
            State = state;
            Parent = parent;
            Child = child;
        }

        public EntityState State;

        public object Parent;

        public object Child;
    }
}
