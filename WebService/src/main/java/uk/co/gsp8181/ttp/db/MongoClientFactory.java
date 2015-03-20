package uk.co.gsp8181.ttp.db;

import com.mongodb.*;
import org.mongodb.morphia.Datastore;
import org.mongodb.morphia.Morphia;

import java.net.UnknownHostException;

/**
 * Created by b1020537 on 20/03/2015.
 */
public class MongoClientFactory {

    private static final MongoClientFactory INSTANCE = new MongoClientFactory();
    private final Datastore datastore;
    public final String dbname;
    public boolean usingOpenshift;

    private MongoClientFactory() {
        usingOpenshift = (System.getenv("OPENSHIFT_MONGODB_DB_HOST") != null);


        MongoClient mongoClient;
        if(!usingOpenshift)
        {
            dbname = "ttp";
            try {
                mongoClient = new MongoClient("localhost");
            } catch (UnknownHostException e) {
                throw new RuntimeException("Forget about running servers mate");
            }
        } else
        {
            dbname = System.getenv("OPENSHIFT_APP_NAME");
            String host = System.getenv("OPENSHIFT_MONGODB_DB_HOST");
            int port = Integer.parseInt(System.getenv("OPENSHIFT_MONGODB_DB_PORT"));
            String username = System.getenv("OPENSHIFT_MONGODB_DB_USERNAME");
            String password = System.getenv("OPENSHIFT_MONGODB_DB_PASSWORD");
            MongoClientOptions mongoClientOptions = MongoClientOptions.builder().build();
            try {
                mongoClient = new MongoClient(new ServerAddress(host, port), mongoClientOptions);
            } catch (UnknownHostException e) {
                throw new RuntimeException("Openshift was detected but a mongo instance was not found");
            }
            mongoClient.setWriteConcern(WriteConcern.SAFE);
        }
        datastore = new Morphia().createDatastore(mongoClient, dbname);
        datastore.ensureIndexes();
        datastore.ensureCaps();
        //LOG.info("Connection to database '" + DB_HOST + ":" + DB_PORT + "/" + DB_NAME + "' initialized");


    }


    public static MongoClientFactory getInstance()
    {
        return INSTANCE;
    }

    public Datastore getDatastore()
    {
        return datastore;
    }
}
