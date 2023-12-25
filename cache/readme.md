### Definition
In order to perform database operations that will be used extensively within the scope of Amorphie projects, 2 architectural structures have been emphasized. This folder was created to compare redis and couchbase.
As an example, couchbase to kafka and kafka to couchbase were tested under the couchbase folder.
Under the redis folder, a kafka to redis cache test was made. Below is a brief comparison of couchbase and redis.

1. Data Model: Couchbase allows you to store complex data with flexible structures using JSON documents. Redis, on the other hand, is primarily focused on simple key-value storage and has limited support for complex data structures.

2. Consistency and Durability: Couchbase ensures that data is consistent and durable through features like replication and automatic failover. Redis, by default, provides eventual consistency, which means there can be some delay in data synchronization, and additional configuration is needed for replication and durability.

3. Scalability: Couchbase is designed to handle high scalability by distributing data across multiple nodes in a cluster. It has built-in mechanisms for dividing data and balancing the load. Redis also supports horizontal scaling, but it requires external solutions to achieve distributed setups.

4. Querying and Indexing: Couchbase provides a powerful query language called N1QL (SQL for JSON) that allows for complex searches and indexing on JSON data. Redis, on the other hand, does not provide a built-in querying language and primarily relies on key-value operations.

5. Caching and Persistence: Couchbase offers a combination of in-memory caching and persistent storage. It allows you to define cache eviction policies and provides flexibility in storing data in memory or on disk. Redis, on the other hand, is commonly used as an in-memory cache but does not provide built-in persistence.

6. Integrated Full-Text Search: Couchbase has a built-in full-text search engine called Couchbase Full-Text Search. It enables you to perform advanced searches on textual data within JSON documents without the need for external search engines. Redis does not have built-in full-text search capabilities.

7. Multi-Datacenter Replication: Couchbase supports replicating data across multiple datacenters, which is useful for disaster recovery, data distribution, and improving performance by locating data closer to users. Redis does not provide native support for cross-datacenter replication.
