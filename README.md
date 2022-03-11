# AVUP.Fun

基于[Confluent Kafka](https://www.confluent.io)与[ClickHouse](https://clickhouse.tech)的数据仓库搭建

<details>
<summary>样例搭建</summary>
1. 创建Kafka主题

```shell
kafka-topics --bootstrap-server localhost:9092 --create --topic live --partitions 1
kafka-topics --bootstrap-server localhost:9092 --create --topic pending --partitions 6
kafka-topics --bootstrap-server localhost:9092 --create --topic missing --partitions 1
kafka-topics --bootstrap-server localhost:9092 --create --topic acer --partitions 6
kafka-topics --bootstrap-server localhost:9092 --create --topic processed --partitions 6
kafka-topics --bootstrap-server localhost:9092 --create --topic hub --partitions 1
kafka-topics --bootstrap-server localhost:9092 --create --topic room --partitions 1
```
2. 创建ClickHouse数据库

```sql
create database acfun;
```
3. 创建live表
```sql
create table acfun.live (
    `UserId` UInt64,
    `LiveId` String,
    `Title` String,
    `Like` UInt32,
    `Audience` UInt32,
    `TypeId` UInt16,
    `TypeCategory` UInt16,
    `TypeName` String,
    `TypeCategoryName` String,
    `UserPost` UInt32,
    `UserFan` UInt32,
    `UserFollowing` UInt32,
    `UserAvatar` String,
    `UserName` String,
    `CreateTime` DateTime64,
    `Timestamp` DateTime64
) ENGINE = MergeTree() ORDER BY (UserId, LiveId);

create table acfun.kafka_live (
    `UserId` UInt64,
    `LiveId` String,
    `Title` String,
    `Like` UInt32,
    `Audience` UInt32,
    `TypeId` UInt16,
    `TypeCategory` UInt16,
    `TypeName` String,
    `TypeCategoryName` String,
    `UserPost` UInt32,
    `UserFan` UInt32,
    `UserFollowing` UInt32,
    `UserAvatar` String,
    `UserName` String,
    `CreateTime` DateTime64,
    `Timestamp` DateTime64
) ENGINE = Kafka SETTINGS
kafka_broker_list = 'broker:9092',
kafka_topic_list = 'live',
kafka_group_name = 'live',
kafka_format = 'JSONEachRow';

create materialized view acfun.live_view TO acfun.live AS SELECT * FROM acfun.kafka_live;
```
4. 创建acer表

```sql
create table acfun.acer (
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
) ENGINE = MergeTree() ORDER BY (UperId, LiveId, UserId);

create table acfun.kafka_acer (
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
) ENGINE = Kafka SETTINGS
kafka_broker_list = 'broker:9092',
kafka_topic_list = 'acer',
kafka_group_name = 'acer',
kafka_format = 'JSONEachRow',
kafka_num_consumers = 6;

create materialized view acfun.acer_view TO acfun.acer AS SELECT * FROM acfun.kafka_acer;
```

5. 创建room表

```sql
create table acfun.room (
    `UserId` UInt64,
    `LiveId` String,
    `Timestamp` DateTime64,
    `Banana` UInt64,
    `Like` UInt64,
    `LikeDelta` Int32,
    `Audience` UInt64
) ENGINE = MergeTree() ORDER BY (UserId, LiveId);

create table acfun.kafka_room (
    `UserId` UInt64,
    `LiveId` String,
    `Timestamp` DateTime64,
    `Banana` UInt64,
    `Like` UInt64,
    `LikeDelta` Int32,
    `Audience` UInt64
) ENGINE = Kafka SETTINGS
kafka_broker_list = 'broker:9092',
kafka_topic_list = 'room',
kafka_group_name = 'room',
kafka_format = 'JSONEachRow';

create materialized view acfun.room_view TO acfun.room AS SELECT * FROM acfun.kafka_room;
```

6. 创建processed表

```sql
create table acfun.processed (
    `UperId` UInt64,
    `LiveId` String,
    `MessageType` String,
    `Payload` String
) ENGINE = MergeTree() ORDER BY (UperId, LiveId);

create table acfun.kafka_processed (
    `UperId` UInt64,
    `LiveId` String,
    `MessageType` String,
    `Payload` String
) ENGINE = Kafka SETTINGS
kafka_broker_list = 'broker:9092',
kafka_topic_list = 'processed',
kafka_group_name = 'processed',
kafka_format = 'JSONEachRow',
kafka_num_consumers = 6;

create materialized view acfun.processed_view TO acfun.processed AS SELECT * FROM acfun.kafka_processed;
```

7. 向Kafak的acer主题中写入序列化的JSON，内容为

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

## 获取最新开播的主播（默认显示20个）
* https://api.avup.fun/uper?offset=0&limit=20

## 获取主播最新的开播记录（默认显示20个）
* https://api.avup.fun/uper/{id}?offset=0&limit=20

## 获取直播数据（包括弹幕、进入、点赞、关注及礼物）
* https://api.avup.fun/live/{id}/{liveId}?{timestamp}

## 获取直播数据（指定弹幕、进入、点赞、关注或礼物）
* https://api.avup.fun/live/{id}/{liveId}/{type}?{timestamp}

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
