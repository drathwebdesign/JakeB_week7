using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using JetBrains.Annotations;
using UnityEngine;

public class ProductivityUnit : Unit
{
        //current resource pile
    ResourcePile m_CurrentPile = null;
        //multiplier
    public float ProductivityMultiplier = 2;


    //Method triggered when unit is in range
    protected override void BuildingInRange() {
        // Check if we are not already interacting with a pile
        if (m_CurrentPile == null) {
            // Cast the target to a ResourcePile and check if it's valid
            ResourcePile pile = m_Target as ResourcePile;

            // If it is a valid resource pile
            if (pile != null) {
                // Assign the pile to m_CurrentPile
                m_CurrentPile = pile;
                m_CurrentPile.ProductionSpeed *= ProductivityMultiplier;
            }
        }
    }

    //Reset the productivity value of pile if unit leaves
    void ResetProductivity() {
        if (m_CurrentPile != null) {
            m_CurrentPile.ProductionSpeed /= ProductivityMultiplier;
            m_CurrentPile = null;
        }
    }

    public override void GoTo(Building target) {
        ResetProductivity();
        base.GoTo(target);
    }

    public override void GoTo(Vector3 position) {
        ResetProductivity();
        base.GoTo(position);
    }
}