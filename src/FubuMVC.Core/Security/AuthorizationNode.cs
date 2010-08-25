using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using System.Linq;

namespace FubuMVC.Core.Security
{
    public class AuthorizationNode : BehaviorNode
    {
        private readonly ListDependency _policies = new ListDependency(typeof(IEnumerable<IAuthorizationPolicy>));

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Authorization; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var objectDef = new ObjectDef(typeof(AuthorizationBehavior));
            objectDef.Dependencies.Add(_policies);

            return objectDef;
        }

        public AllowRole AddRole(string roleName)
        {
            var allow = new AllowRole(roleName);

            _policies.AddValue(allow);


            return allow;
        }

        public IEnumerable<string> AllowedRoles()
        {
            return _policies.Items.Where(x => x.Value is AllowRole).Select(x => x.Value.As<AllowRole>().Role);
        }

        // To any followers here, there definitely needs to be more stuff here, but roles are
        // all Dovetail needs at this moment, so...
        public bool HasRules()
        {
            return _policies.Items.Any();
        }

        public ObjectDef ToEndpointAuthorizorObjectDef()
        {
            var objectDef = new ObjectDef(typeof(EndPointAuthorizor)){
                Name = ParentChain().UniqueId.ToString()
            };

            objectDef.Dependencies.Add(_policies);

            return objectDef;
        }

        public void Register(Guid uniqueId, Action<Type, ObjectDef> callback)
        {
            var objectDef = HasRules() 
                ? ToEndpointAuthorizorObjectDef()
                : new ObjectDef(){
                    Value = NulloEndPointAuthorizor.Flyweight
                };

            objectDef.Name = uniqueId.ToString();

            callback(typeof (IEndPointAuthorizor), objectDef);

        }
    }
}