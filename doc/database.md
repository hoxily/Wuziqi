## users表
users表用于记录注册用户信息

创建该表所需SQL语句为：

    create table users
    (
        id int not null auto_increment unique key,
        password char(40) not null,
        email varchar(128) not null,
        name varchar(16) null,
        birthday date null,
        qq varchar(16) null,
        mobilephone char(11) null,
        signature varchar(128) null,
        registertime datetime null
    );

