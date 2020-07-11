using System.Collections.Generic;

/**
 * Any agent that wants to use GOAP must implement
 * this interface. It provides information to the GOAP
 * planner so it can plan what actions to use.
 * 
 */
public interface IGoap
{
	/**
	 * The starting state of the Agent and the world.
	 * Supply what states are needed for actions to run.
	 */
	Dictionary<string, bool> GetWorldState ();

	/**
	 * Give the planner a new goal so it can figure out 
	 * the actions needed to fulfill it.
	 */
	Dictionary<string, bool> GetGoalState(int index);
	
	int GetGoalsCount();
	
	/**
	 * Called during Update. Move the agent towards the target in order
	 * for the next action to be able to perform.
	 */
	void MoveAgent(GoapAction nextAction);


}

