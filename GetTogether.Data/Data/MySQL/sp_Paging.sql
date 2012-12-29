DELIMITER $$

DROP PROCEDURE IF EXISTS `sp_Paging` $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_Paging`(
in PageIndex      int,
in FieldsShow       varchar(500),
in TableName     varchar(500),
in Conditions    varchar(500),
in FieldsOrder   varchar(100),
in IsDesc     int,
in PrimaryKeys varchar(100),
in PageSize      int,
in QueryType     int
)
begin
    declare sTemp  varchar(1000);
    declare sSql   varchar(4000);
    declare sSqlCounter varchar(4000);
    declare sOrder varchar(1000);
    if QueryType is null or QueryType='' then
        set QueryType=0;
    end if;

    if QueryType = 0 then
        if IsDesc = 1 then
            set sOrder = concat(' order by ', FieldsOrder, ' desc ');
            set sTemp  = '<(select min';
        else
            set sOrder = concat(' order by ', FieldsOrder, ' asc ');
            set sTemp  = '>(select max';
        end if;

        if PageIndex = 1 then
            if Conditions <> '' then
                set sSql = concat('select ', FieldsShow, ' from ', TableName, ' where ');
                set sSql = concat(sSql, Conditions, sOrder, ' limit ?');
            else
                set sSql = concat('select ', FieldsShow, ' from ', TableName, sOrder, ' limit ?');
            end if;
        else
            if Conditions <> '' then
                set sSql = concat('select ', FieldsShow, ' from ', TableName);
                set sSql = concat(sSql, ' where ', Conditions, ' and ', PrimaryKeys, sTemp);
                set sSql = concat(sSql, '(', PrimaryKeys, ')', ' from (select ');
                set sSql = concat(sSql, ' ', PrimaryKeys, ' from ', TableName, sOrder);
                set sSql = concat(sSql, ' limit ', (PageIndex-1)*PageSize, ') as tabtemp)', sOrder);
                set sSql = concat(sSql, ' limit ?');
            else
                set sSql = concat('select ', FieldsShow, ' from ', TableName);
                set sSql = concat(sSql, ' where ', PrimaryKeys, sTemp);
                set sSql = concat(sSql, '(', PrimaryKeys, ')', ' from (select ');
                set sSql = concat(sSql, ' ', PrimaryKeys, ' from ', TableName, sOrder);
                set sSql = concat(sSql, ' limit ', (PageIndex-1)*PageSize, ') as tabtemp)', sOrder);
                set sSql = concat(sSql, ' limit ?');
            end if;
        end if;
            if Conditions <> '' then
                set sSqlCounter = concat('select count(1) from ', TableName, ' where ',Conditions);
            else
                set sSqlCounter = concat('select count(1) from ', TableName);
            end if;
    else
        if IsDesc = 1 then
            set sOrder = concat(' order by ', FieldsOrder, ' desc ');
            set sTemp  = '<(select min';
        else
            set sOrder = concat(' order by ', FieldsOrder, ' asc ');
            set sTemp  = '>(select max';
        end if;

        if PageIndex = 1 then
            if Conditions <> '' then
                set sSql = concat('select ', FieldsShow, ' from (', TableName, ') as PagingTable where ');
                set sSql = concat(sSql, Conditions, sOrder, ' limit ?');
            else
                set sSql = concat('select ', FieldsShow, ' from (', TableName,') as PagingTable ', sOrder, ' limit ?');
            end if;
        else
            if Conditions <> '' then
                set sSql = concat('select ', FieldsShow, ' from (', TableName,') as PagingTable');
                set sSql = concat(sSql, ' where ', Conditions, ' and ', PrimaryKeys, sTemp);
                set sSql = concat(sSql, '(', PrimaryKeys, ')', ' from (select ');
                set sSql = concat(sSql, ' ', PrimaryKeys, ' from (', TableName,') as PagingTable where ',Conditions,' ', sOrder);
                set sSql = concat(sSql, ' limit ', (PageIndex-1)*PageSize, ') as tabtemp)', sOrder);
                set sSql = concat(sSql, ' limit ?');
            else
                set sSql = concat('select ', FieldsShow, ' from (', TableName,') as PagingTable');
                set sSql = concat(sSql, ' where ', PrimaryKeys, sTemp);
                set sSql = concat(sSql, '(', PrimaryKeys, ')', ' from (select ');
                set sSql = concat(sSql, ' ', PrimaryKeys, ' from (', TableName,') as PagingTable', sOrder);
                set sSql = concat(sSql, ' limit ', (PageIndex-1)*PageSize, ') as tabtemp)', sOrder);
                set sSql = concat(sSql, ' limit ?');
            end if;
        end if;
            if Conditions <> '' then
                set sSqlCounter = concat('select count(1) from (', TableName, ') as PagingTable where ',Conditions);
            else
                set sSqlCounter = concat('select count(1) from (', TableName,') as PagingTable');
            end if;
    end if;

    set @iPageSize = PageSize;
    set @sQuery = sSql;
    prepare stmt from @sQuery;
    execute stmt using @iPageSize;



    set @sQueryCounter=sSqlCounter;
    prepare stmt from @sQueryCounter;
    execute stmt;

    #select sSql;



end $$

DELIMITER ;