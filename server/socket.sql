

  alter table [User] add status varchar(10)

  create table Log(
  id int identity(1,1) primary key,
  L_date datetime not null,
  describe varchar(max), --记录简介
  )

  --创建用户上线下线日志触发器
  create trigger trigger_log
  on [User]
  after update
  as
  begin
   --取用户名
	declare @username varchar(max)
	--取登陆状态
	declare @status varchar(max)

	--分开一行一行读取操作
	declare @i int
	declare @rows int
	select @rows = count(*) from inserted
	select @i=1
	while(@i<=@rows)
		  begin
			 select top (@i) @username = username,@status = status from inserted where userid not in (select top (@i-1) userid from inserted)
			 select @i+=1
			 --判断是上线操作还是下线操作
			 if(@status = 'on')
				begin
				 insert into Log(L_date,describe) values(GETDATE(),@username+'上线')
				end
				else if(@status = 'off')
				begin
					 insert into Log(L_date,describe) values(GETDATE(),@username+'下线')
				end
		  end
   end


  select* from [User]
  select * from Log

  delete from Log

  update [User] set status = 'off' 

