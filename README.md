# AVUP.Fun

基于[Confluent Kafka](https://www.confluent.io)与[ClickHouse](https://clickhouse.tech)的数据仓库搭建

<details>
<summary>样例搭建</summary>
1. 创建Kafka主题

```shell
kafka-topics --bootstrap-server localhost:9092 --create --topic acer
```
2. 创建ClickHouse数据库

```sql
create database acfun;
```
3. 创建acer表
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
4. 创建kafka_acer表，用于连接kafka的acer主题

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

5. 创建acer_view表，用于从kafka_acer表中导入数据至acer表

```sql
CREATE MATERIALIZED VIEW acfun.acer_view TO acfun.acer AS
SELECT *
FROM acfun.kafka_acer
```

6. 向Kafak的acer主题中写入序列化的JSON，内容为

```
{
    "UperId": number,
    "LiveId": "string",
    "Type": "string",
    "UserId": number,
    "UserName": "string",
    "UserAvatar": "string",
    "UserData": "string",
    "UserBadgeUperId": number,
    "UserBadgeName": "string",
    "UserBadgeLevel": number,
    "UserManagerType": number,
    "Timestamp": number,
    "Comment": "string",
    "GiftId": number,
    "GiftName": "string",
    "GiftCount": number,
    "GiftCombo": number,
    "GiftComboId": "string",
    "GiftValue": number
}
```
</details>

<details>
<summary>现有API</summary>

# 非官方不完整统计

## 获取最新开播的主播（默认显示10个）
* https://api.avup.fun/uper
* https://api.avup.fun/uper/{offset}/{limit}

## 获取主播最新的开播记录（默认显示10个）
* https://api.avup.fun/uper/{id}
* https://api.avup.fun/uper/{id}/{offset}/{limit}

## 获取直播数据（包括弹幕、进入、点赞、关注及礼物）
* https://api.avup.fun/live/{id}/{liveId}
* https://api.avup.fun/live/{id}/{liveId}/{timestamp}

## 获取直播数据（指定弹幕、进入、点赞、关注或礼物）
* https://api.avup.fun/live/{id}/{liveId}/{type}
* https://api.avup.fun/live/{id}/{liveId}/{type}/{timestamp}

**type: comment, enter, like, follow, gift**

## 获取当日/周/月观看直播最多的10位观众
* https://api.avup.fun/statistics/live/day
* https://api.avup.fun/statistics/live/week
* https://api.avup.fun/statistics/live/month

## 获取当日/周/月发送弹幕最多的10位观众
* https://api.avup.fun/statistics/comment/day
* https://api.avup.fun/statistics/comment/week
* https://api.avup.fun/statistics/comment/month
</details>
