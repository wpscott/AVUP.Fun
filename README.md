# AVUP.Fun

����[Confluent Kafka](https://www.confluent.io)��[ClickHouse](https://clickhouse.tech)�����ݲֿ�

<details>
<summary>�����</summary>
1. ����Kafka����

```shell
kafka-topics --bootstrap-server localhost:9092 --create --topic acer
```
2. ����ClickHouse���ݿ�

```sql
create database acfun;
```
3. ����acer��
```sql
CREATE TABLE acfun.acer
(
    `UperId` UInt64,
    `LiveId` String,
    `Type` String,
    `UserId` UInt64,
    `UserName` String,
    `UserAvatar` String,
    `UserData` String,
    `UserBadgeUperId` UInt64,
    `UserBadgeName` String,
    `UserBadgeLevel` UInt8,
    `UserManagerType` UInt8,
    `Timestamp` DateTime64,
    `Comment` String,
    `GiftId` UInt32,
    `GiftName` String,
    `GiftCount` UInt32,
    `GiftCombo` UInt32,
    `GiftComboId` String,
    `GiftValue` UInt64
)
ENGINE = MergeTree()
ORDER BY (UperId, LiveId, UserId)
```
4. ����kafka_acer����������kafka��acer����

```sql
CREATE TABLE acfun.kafka_acer
(
    `UperId` UInt64,
    `LiveId` String,
    `Type` String,
    `UserId` UInt64,
    `UserName` String,
    `UserAvatar` String,
    `UserData` String,
    `UserBadgeUperId` UInt64,
    `UserBadgeName` String,
    `UserBadgeLevel` UInt8,
    `UserManagerType` UInt8,
    `Timestamp` DateTime64,
    `Comment` String,
    `GiftId` UInt32,
    `GiftName` String,
    `GiftCount` UInt32,
    `GiftCombo` UInt32,
    `GiftComboId` String,
    `GiftValue` UInt64
)
ENGINE = Kafka
SETTINGS kafka_broker_list = 'broker:9092', kafka_topic_list = 'acer', kafka_group_name = 'acer', kafka_format = 'JSONEachRow'
```

5. ����acer_view�����ڴ�kafka_acer���е���������acer��

```sql
CREATE MATERIALIZED VIEW acfun.acer_view TO acfun.acer AS
SELECT *
FROM acfun.kafka_acer
```

6. ��Kafak��acer������д�����л���JSON������Ϊ

```json
{
    "UperId": 0,
    "LiveId": "string",
    "Type": "string",
    "UserId": 0,
    "UserName": "string",
    "UserAvatar": "string",
    "UserData": "string",
    "UserBadgeUperId": 0,
    "UserBadgeName": "string",
    "UserBadgeLevel": 0,
    "UserManagerType": 0,
    "Timestamp": 0,
    "Comment": "string",
    "GiftId": 0,
    "GiftName": "string",
    "GiftCount": 0,
    "GiftCombo": 0,
    "GiftComboId": "string",
    "GiftValue": 0
}
```
</details>

<details>
<summary>����API</summary>

# �ǹٷ�������ͳ��

## ��ȡ���¿�����������Ĭ����ʾ10����
* https://api.avup.fun/uper
* https://api.avup.fun/uper/{offset}/{limit}

## ��ȡ�������µĿ�����¼��Ĭ����ʾ10����
* https://api.avup.fun/uper/{id}
* https://api.avup.fun/uper/{id}/{offset}/{limit}

## ��ȡֱ�����ݣ�������Ļ�����롢���ޡ���ע�����
* https://api.avup.fun/live/{id}/{liveId}
* https://api.avup.fun/live/{id}/{liveId}/{timestamp}

## ��ȡֱ�����ݣ�ָ����Ļ�����롢���ޡ���ע�����
* https://api.avup.fun/live/{id}/{liveId}/{type}
* https://api.avup.fun/live/{id}/{liveId}/{type}/{timestamp}

**type: comment, enter, like, follow, gift**

## ��ȡ����/��/�¹ۿ�ֱ������10λ����
* https://api.avup.fun/statistics/live/day
* https://api.avup.fun/statistics/live/week
* https://api.avup.fun/statistics/live/month

## ��ȡ����/��/�·��͵�Ļ����10λ����
* https://api.avup.fun/statistics/comment/day
* https://api.avup.fun/statistics/comment/week
* https://api.avup.fun/statistics/comment/month
</details>