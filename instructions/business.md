## overview:

نظام لمعارض السيارات و ادارتها من تأجير السيارات بعقودها القصيرة والطويلة وبجميع بياناتها و تقريرها

## reuirments:

authintication and authorization :
1- supported by jwt schema
2- users table with roles table then userrole table
3- register and login with email and password
4- forget and reset password
5- logout
6- refresh token 

يتم تسجيل السيارة بهذه البيانات : crud operations

1- نوع السيارة etc honda ..
2- موديل صنع السيارة etc : 2020
3- موديل السيارة etc : accord
4- اللون
5- رقم اللوحة
6-  الفرع
7- تاريخ الشراء 
8- سعر الشراء 
9- رابط الصورة
10- رقم الشاسيه
11- رقم المحرك
12- ملاحظات


يتم تسجيل مستخدم السيارة بهذه البيانات (العملاء) : crud operations

1- اسم العميل
2- رقم هوية العميل
3- رقم جوال العميل
4- عنوان العميل


مواقع المعرض و فروعه : crud operations

1- اسم الفرع
2- عنوان الفرع
3- رقم جوال الفرع


انواع العقود :  (lookup) readonly without any crud operations

1- ايجار خدمة
2- ايجار قصير الاجل او حسب الطلب (سنة او من سنة )
3- ايجار طويل الاجل (سنة فأكثر )
4- ايجار منتهي بالتمليك 


very instructions : 
- you can make car type and model and year lookup and dependent dropdown list and the dropdownlist supported by typesearch debounce search and loading indicator with no flicker effect
- use caching static lookup data if you want or anything if you want to acheive best performance
- technologies used: use .net last and stable version for backend, use vue for frontend best version also , use postgresql for database
- use best performance for frontend and backend
- use ready free template for vue js like YummyAdmin, tailwind-admin-vue3, Kunafa Dashboard Vue, CoreUI Vue Admin and etc like that > make it reponsive and localized arabic and english and i think the template support that so search and download it
- use best practices for coding
- write clean code
- clean and great and good performance architacture
- use best tools and best practices for software development and software engineer (patterns and everythings)
- make images for cars uploaded by the system, and store images files at wwwroot and store at DB just the path for it


git at github here:

git init
git add README.md
git commit -m "first commit"
git branch -M main
git remote add origin https://github.com/Mohammed99Ismail/car.git
git push -u origin main



