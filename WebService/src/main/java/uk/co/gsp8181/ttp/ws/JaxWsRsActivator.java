package uk.co.gsp8181.ttp.ws;

import javax.ws.rs.ApplicationPath;
import javax.ws.rs.core.Application;

/**
 * Required class for using Java-EE type servers (TomEE/JBoss)
 * @author Geoffrey Prytherch <gsp8181@users.noreply.github.com>
 * @since 2015-03-19
 */
@ApplicationPath("/rest")
public class JaxWsRsActivator extends Application { }
