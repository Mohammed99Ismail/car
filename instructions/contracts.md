## overview:
we make contracts for any car at the car showroom, the contract supported by the 4 types we have, 
- عقد خدمة
- عقد ايجار قصير الاجل او حسب الطلب (سنة او من سنة )
- عقد ايجار طويل الاجل (سنة فأكثر )
- عقد ايجار منتهي بالتمليك 

## car table addational notes:
- every car has field named القيمة الدفترية (the book value of the car) new field at database

هذه الملاحظات في حال تأجير السيارة يتم تطبيقها
- يتم اهلاك السيارة خلال اربع سنوات فقط وبعد مرورها تكون القيمة الدفترية ب 1 فقط 
- يتم اهلاك السيارة بمعدل 25% سنويا
- يتم حساب القيمة الدفترية بناء على تاريخ دخول السيارة للمعرض



*******************

## common fields of all contracts:
1- contract id
2- contract type
3- car id
4- customer id
5- branch id
6- start date
7- end date
8- total price
9- paid price
10- remaining price
11- notes

## عقد خدمة:
required conditions:
1- car is owned by the showroom employee/user (customer dont own the car)
2- There is no down payment and no installments or anything, just system user (employee) and car and the data start and end (rent period if by months or days or weeks) 

## عقد ايجار قصير الاجل او حسب الطلب:
required conditions:
1- car is owned by customer
2- For every car that is rented, a specific mileage limit will be set and if the customer exceeds this limit, there will be a fees for every kilometer over the limit like 0.50 SAR for every kilometer exceeds and the fees will be added to the total rent amount at the end of the contract
3- should be 12 months or less only
4- no down payemnt, just Monthly rent
5- there are 4 terms deducted from the total rent amount:
    1- التأمين الشامل
    2- الصيانة
    3- سيارة بديلة
    4- مصاريف إدارية 
    
فيه اهلاكات و تقارير محسوبة و كاتبهالك في ملف 
C:\Projects\car\instructions\contracts_examples.md

## عقد ايجار طويل الاجل:
required conditions:
1- car is owned by customer
2- For every car that is rented, a specific mileage limit will be set and if the customer exceeds this limit, there will be a fees for every kilometer over the limit like 0.50 SAR for every kilometer exceeds and the fees will be added to the total rent amount at the end of the contract
3- should be minimum 12 months one year and can be more than year
4- has ability to make down payment and monthyly rent
5- there are 4 terms deducted from the total rent amount:
    1- التأمين الشامل
    2- الصيانة
    3- سيارة بديلة
    4- مصاريف إدارية 

فيه اهلاكات و تقارير محسوبة و كاتبهالك في ملف 
C:\Projects\car\instructions\contracts examples.md

## عقد ايجار منتهي بالتمليك:
required conditions:
1- car is owned by customer
2- has ability to make down payment and monthyly rent
3- no 4 terms deducted











The 4 terms we can make a config page or modal to set it's values globally then applied automatically to the contract which have those terms:
1- التأمين الشامل
2- الصيانة
3- سيارة بديلة
4- مصاريف إدارية   

and for every contract which have those terms applied auto this values but you can edit it manualy in the contract.
