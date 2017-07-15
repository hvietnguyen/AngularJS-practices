use [Cobra-onboarding-db]

select oh.OrderId,oh.OrderDate,c.Name,p.ProductName,p.Price
from People c 
inner join OrderHeaders oh on c.Id=oh.PersonId
inner join OrderDetails od on oh.OrderId=od.OrderId
inner join Products p on p.Id=od.ProductId

select * from Products
select * from People
select * from OrderHeaders
select * from OrderDetails

Delete from OrderDetails where Id=21 
Delete from OrderHeaders where OrderId=21

Insert into OrderHeaders(OrderDate,PersonId)
Values('2017-07-05',2)

Insert into OrderDetails(OrderId,ProductId)
Values (15,2)