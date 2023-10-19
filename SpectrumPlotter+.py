# -*- coding: utf-8 -*-
"""
Created on Fri Jul 14 11:24:53 2023
@author: jordan.walsh
@author: owen.moynihan

"""

from glob import glob
import numpy as np
import matplotlib.pyplot as plt 
import os
from pylab import *
import sys

index = 0
peaks = []
data = []
h = 6.62607015e-34 #planck
c = 2.99792458e8 #speed of light
e = 1.60217663e-19 #electron
k = 1.380649e-23 #boltzmann
J_eV = 1/(6.242e18) #Joules / eV
i = 0

#------------------------------- PLOT CONTROLS -------------------------------#

path =r"/home/salty/code files/testFilesSpectrum"
PLOT_LIMIT = [380, 460]#400, 500] # wavelength limits in nm
Y_LIMIT = [1e-3, 2] # enforced only for log plots [lower, upper] - leave blank for auto

INCLUDE_ENERGY_SCALE = True # Wavelegnth & energy plot - energy is indication only, intensity not accounted for
PLOT_INDIVIDUAL_SPECTRUMS = False # Plot all files individually
CREATE_COMPILED_PLOT = True # Plot all spectrums in one

ENERGY_ONLY = True # Energy (eV) only mode || overrides INCLUDE_ENERGY_SCALE if enabled and rescales to reflect true intensity of energy
ALL_PLOTS_MAX_1 = False # Plots with all spectrums will be normalised to have the same maximum (1)

LOG_PLOT_FITTING = True # [Carrier Temp] Return information for boltzmann stats from specified FIT_RANGE [Least Squares fit]
FIT_RANGE = [385, 400] # wavelength in nm || These options allow you to find carrier temperature for ENERGY_ONLY plots

#-----------------------------------------------------------------------------#

# WARNING: TICKS ON BOTTOM AND TOP MUST BE ALLIGNED FOR ENERGY SCALE TO BE DISPLAYED CORRECTLY

#-----------------------------------------------------------------------------#

def preventDivisionByZero(some_array):
    corrected_array = some_array.copy()
    for i, entry in enumerate(some_array):
        # If element is zero, set to some small value
        if abs(entry) < sys.float_info.epsilon:
            corrected_array[i] = sys.float_info.epsilon
    return corrected_array

# Converting wavelength (nm) to energy (eV)
def WLtoE(wl, label):
    if str(type(wl)) == "<class 'list'>": #check for single value or list of values
        # Prevent division by zero error
        for i in range(len(wl)):
            wl[i] = float(wl[i])
        wl = preventDivisionByZero(wl)
        wl = np.array(wl)
        
    E_eV = 1243 / wl #h*c=1243
    if label == 'label': #what is the purpose of conversion (i.e for scale labels or important data)
        for i in range(len(E_eV)):
            E_eV[i] = float("{:.2f}".format(E_eV[i]))
    return E_eV

def add_energy_scale(plt): #converts photon wavelength to energy, adds scale on top - does not account for reltive intensity change
    ax = plt.gca()
    labels = ax.get_xticks()
    ax2 = ax.twiny()
    ax2.set_xlim(PLOT_LIMIT)
    ax2.set_xticks(labels, WLtoE(labels, 'label')) #locations, corresponding labels
    ax2.set_xlabel('Energy (eV)', fontsize=8)

def convert_wl_intensity_to_energy_intensity(y, x): #x must be wl in nm
    i = 0
    try: #if fails its because index is out of range (y isnt an array/list)
        y = np.array(y)
        x = np.array(x)
        for i in range(len(y)):
            y[i] = y[i] * (x[i]*1e-9)^2 / (h*c)
    except Exception as err: #I am aware that this is the most lazy way possible to do this
        #print("conversion not array [{}]".format(err))
        y = y * (x*1e-9)**2 / (h*c)
    return y

def plot_graph(x, y, top_energy_scale, energy_scale_only): #create a single plot
    labels = []
    plt.figure()
    if energy_scale_only == True:
        y = convert_wl_intensity_to_energy_intensity(y, x)
        x = WLtoE(x, 'data')
        plt.xlim(WLtoE(PLOT_LIMIT, 'data'))
    else:
        plt.xlim(PLOT_LIMIT)
    plt.plot(x, y, linewidth = 1, color = '#0c00b4ff')
    plt.title(f"")
    plt.xlabel("Wavelength (nm)", fontsize=8)
    plt.ylabel("Intensity (au)",  fontsize=8)
    plt.xticks(fontsize = 7)
    plt.yticks(fontsize = 7)
    
    matplotlib.pyplot.figtext(0.6,0.8, f"Max = {maximum}nm", fontsize = "medium")
    plt.grid(True, alpha=0.5)
    labels = np.array(labels)
    if top_energy_scale == True and energy_scale_only == False:
        add_energy_scale(plt)
    plt.savefig(f'{file}_figure.png', dpi = 1000, bbox_inches='tight')
    plt.show()
    
def return_boltzmann(x, y): #extract exponential features between bounds elected
    x = np.array(x)
    y = np.array(y)
    
    try:
        if ENERGY_ONLY == True:
            E_FIT = WLtoE(FIT_RANGE, 'data')
            indices = np.where((x >= min(E_FIT)) & (x <= max(E_FIT)))
        else:
            indices = np.where((x >= min(FIT_RANGE)) & (x <= max(FIT_RANGE))) #extrapolate range for fitting
        if indices[0].size < 1:
            raise Exception("Failed to find indices in range provided")
    except Exception as err:
        print("ERROR [{}]".format(err))
    
    x_fit = x[indices]
    y_fit = np.log(y[indices])
    deg = 1    
    p, cov = np.polyfit(x_fit, y_fit, deg, full=False, cov=True)    
    a = np.exp(p[deg])  
    b = p[0]
    poly_error = sqrt(diag(cov)[1])
    print(p)
    print(cov)
    print(poly_error)
    
    x_fitted = np.linspace(np.min(x_fit), np.max(x_fit), 100)
    y_fitted = a*np.exp(b*x_fitted)
    
    return x_fitted, y_fitted, b, poly_error

def plot_all(All_files, plot_type, energy_scale, legend, y_all): #create a compiled plot 
    for file in All_files:                               
          # Create the filepath of particular file
          file_path =f"{path}/{file}"
          #plt.figure()
          with open(file_path) as f:
              lines = f.readlines()[14:]
              x = [float(line.split()[0]) for line in lines]
              y = [float(line.split()[1]) for line in lines]
              y_norm = y
              if ENERGY_ONLY == True:
                  y_norm = convert_wl_intensity_to_energy_intensity(y_norm, x) #x must be in nm ####ISSUE HERE
                  x = WLtoE(x, 'data')
                  
              if ALL_PLOTS_MAX_1 == True:
                  fit = max(y)
                  for n in range(len(y_norm)):
                      y_norm[n] = y_norm[n] / fit
              else:
                  for n in range(len(y_norm)):
                      y_norm[n] = y_norm[n] / max(y_all)
          plt.plot(x,y_norm, linewidth = 1)

    #plot
    plt.ylabel("Intensity (au)",  fontsize=8)
    plt.xticks(fontsize = 10)
    plt.yticks(fontsize = 10)
    plt.grid(True, alpha=0.5)
    plt.legend(legend, bbox_to_anchor = (2,0.35))
    
    if ENERGY_ONLY == True:
        plt.xlim(WLtoE(PLOT_LIMIT, 'data')) #this was making data jagged beacuse WLtoE was limited to float .2f for labels
        x_label = "Energy (eV)"
    else:
        plt.xlim(PLOT_LIMIT)
        x_label = "Wavelength (nm)"
        
    plt.xlabel(x_label, fontsize=8)
    
    if ENERGY_ONLY == True:
        title_label = 'energy'
    else:
        title_label = 'wavelength'
    
    if energy_scale == True and ENERGY_ONLY == False:
        add_energy_scale(plt)
        
    #this will plot exponential fit on linear scale plot if uncommented
    # if LOG_PLOT_FITTING == True:
    #     x_fit, y_fitted, slope = return_boltzmann (x, y_norm)
    #     plt.plot(x_fit, y_fitted, lw=1)
    # print("slope = " + str(slope))
        
    if plot_type == 'log':
        plt.yscale('log') #change to log
        print("\n[Y_LIMIT = {}]".format(str(bool(Y_LIMIT))))
        if bool(Y_LIMIT) == True:
            plt.ylim(Y_LIMIT)   
        if LOG_PLOT_FITTING == True and ENERGY_ONLY == True:
            x_fit, y_fitted, slope, error = return_boltzmann(x, y_norm) #extrapolate exponential features
            plt.plot(x_fit, y_fitted, lw=2, ls='dashed', color='red') 
            print("\nslope = " + str(slope))
            
            #considers maximum possible error based on cavariance maatrix returned by polyfit()
            T = J_eV /(k * abs(slope))
            T_error_1 = abs(J_eV /(k * abs(slope - error)) - T)
            T_error_2 = abs(J_eV /(k * abs(slope + error)) - T)
            T_error = max(T_error_1, T_error_2)
            print("carrier T = " + str(T) + " " + u"\u00B1" + " " + str(T_error) + " K")
            legend.append("exp({} * {}) \nT = {} ".format(float("{:.2f}".format(slope)), title_label, float("{:.2f}".format(T))) + u"\u00B1" + " {} K".format(float("{:.2f}".format(T_error))))
            plt.legend(legend, bbox_to_anchor = (2,0.35))
            title_label = title_label + "_fitted"
        elif LOG_PLOT_FITTING == True and ENERGY_ONLY == False:
            print("Cannot determine carrier temperature from wavelength scale plot [ENERGY_ONLY = False]")
        plt.savefig('All_Spectrums_log_{}'.format(title_label), dpi = 1000, bbox_inches='tight' )
    else:
        plt.savefig('All_Spectrums_{}'.format(title_label), dpi = 1000, bbox_inches='tight' )
        
    plt.show()  


#-------------------------------------MAIN------------------------------------#

os.chdir(path)
All_files = glob('*.txt')
rc('axes', linewidth=1)
plt.rcParams["font.weight"] = "normal"
plt.rcParams["axes.labelweight"] = "normal"
plt.rcParams["font.family"] = "monospace"

print(All_files)
# MAKES GRAPH OF EACH SPECTRUM
y_all = []
for file in All_files:
      # Create the filepath of particular file
      file_path =f"{path}/{file}"
      
      with open(file_path) as f:
          lines = f.readlines()[14:]
          x = [float(line.split()[0]) for line in lines]
          y = [float(line.split()[1]) for line in lines]
                 
          #finding peak of spectrum
          i = y.index(max(y))
          maximum = x[i]
          if ENERGY_ONLY == True:
              y_all.append(convert_wl_intensity_to_energy_intensity(max(y), x[i]))
          else:
              y_all.append(max(y))
          print(f"{file} - max = " + str(x[i])+" nm")
          
          #CREATING A SCATTER PLOT OF DATA
          if PLOT_INDIVIDUAL_SPECTRUMS == True:
              plot_graph(x, y, INCLUDE_ENERGY_SCALE, ENERGY_ONLY)

#MAKE ONE GRAPH WITH ALL SPECTRUMS
if CREATE_COMPILED_PLOT == True:
    legend = []
    for file in All_files:
        if str(file)[0:2] == "ZM":
            legend.append(str(file)[0:3] + " " + str(file)[4:7] + " " + str(file)[str(file).find("int")+4:str(file).find(".txt")].replace("_", " "))
        else:
            legend.append(str(file).replace("_", " ").replace(".txt", ""))
    
    plot_all(All_files, 'log', INCLUDE_ENERGY_SCALE, legend, y_all)
    plot_all(All_files, 'linear', INCLUDE_ENERGY_SCALE, legend, y_all)