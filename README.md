# GridDomain
[![Build status](https://ci.appveyor.com/api/projects/status/fhmbb4x5cmybnl0d/branch/master?svg=true)](https://ci.appveyor.com/project/andreyleskov/griddomain/branch/master)

Nuget feed: https://ci.appveyor.com/nuget/griddomain

GridDomain is a framework for creating business logic focused on your domain, not infrastructure or implementation details. By introducing simple conventions it allows logic to be hightly extandable and scalable. 

GridDomain is build on top of "Holy Trio" paradigm: domain driven design, event sourcing and command-query responsibility segregation, powered by actor model.  
It enforces usage of aggregates, domain events and process managers (sagas) to create business logic. GridDomain allows developer to focus on domain events and processes, taking care of messaging, routing and clustering regardless domain logic structure.

**Show me the code**

Lets create a simple order aggregate : 

    class Order: Aggregate{
        public int Number {get; private set;}
        public IReadonlyCollection<Item> Items => _items;
        
        private readonly List<Item> _items = new List<Item>();
        private Order(Guid id):base(id){}
        
        public Order(Guid id, int number, params Item[] items):base(id)
        {
            RaiseEvent(new OrderCreated(id, number, items));
        }
        public void AddItem(Item itemToAdd)
        {
            RaiseEvent(new ItemAdded(Id, itemToAdd));
        }
        
        private void Apply (OrderCreated e)
        {
            Number = e.Number;
            Id = e.Id;
            _items = e.Items.ToList();
        }
        
        private void Apply (ItemAdded e)
        {
            _items.Add(e.Item);
        }
    }
    
Aggregate class is not depending on any infrastructure, 
so Order can be created by constructor as usual POCO and tested by common unit tests to validate invariants. 
You can see emit-apply pattern is used to run an aggregate. 

Lets add another one aggregate, a discount. It will calculate discount for an order, based on its internal logic and adding some information from 3d-party partner, not controlled by us.

    public class Discount : Aggregate
    {
        public IDiscountOffer Offer { get;private set; }
        public Guid OrderId {get; private set;}
        public Money BaseDiscount {get;private set;}
        public Money PartnerDiscount {get;private set;}
        
        private Discount (Guid id):base(id){}
        public Discount (Guid id, Guid orderId, IDiscountOffer offer):base(id){
           RaiseEvent(new OrderDiscountCreated(id,offer,order);
        }
        
        public void Calculate(Item item, IPartnerService service)
        {
            var partnerDiscount = service.Calculate(item.SkuId);
            var baseDiscount = new Money(item.Price * 0.1);
            if(baseDiscount + partnerDiscount) 
                throw new DiscountIsNotEvailableException();
            RaiseEvent(PendingDisountCreated(baseDiscount,partnerDiscount));
        }
        
        private void Apply(PendingDiscountCreated e)
        {
           BaseDiscount += e.BaseDiscount;
           PartnerDiscount += e.PartnerDiscount;
        }
        
        private void Apply(OrderDiscountCreated e)
        {
            Offer = offer;
            OrderId = order;
        }
    }
    
Discount aggregate requires a method injection to get instance of IPartnerService. It will be biresolve from DI container in special class
called CommandHandler. 
        
        
        
        




