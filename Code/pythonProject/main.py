# This is a sample Python script.

import sys
import math
from PIL import Image
import cv2 as cv
import numpy
import numpy as np

GLOBAL_COUNTER = 1


def differenceCalculationOnLines(lines, isVertical=True):
    differences = 0
    for i in range(len(lines) - 1):
        if (isVertical):
            differences = differences + (lines[i + 1][0] - lines[i][0])
        else:
            differences = differences + (lines[i + 1][1] - lines[i][1])
    avgDiff = differences / (len(lines) - 1)
    print("Found the avg difference: " + str(avgDiff))
    return avgDiff


# This function returns the supposed upper left and bottom-right points of the chess based on vertical lines
# In the following format [Px0,Py0,Px1,Py1]
def checkVerticalLines(vertical, cdstP):
    coordinates = []
    viable = []
    found = False
    vertical.sort(key=lambda x: x[0])
    for i in range(0, len(vertical)):
        l = vertical[i]
        # -1 Because it would count itself in as well
        c = sum(map(lambda x: (abs(l[3] - x[3]) < 51) & (abs(l[1] - x[1]) < 51), vertical)) - 1
        print(
            str(i) + ": Currently checked vertical line coordinates: x0 = {} , y0 = {}, x1 = {}, y1 = {};  number of similar lines: {}"
            .format(l[0],
                    l[1],
                    l[2],
                    l[3], c))


        # Let's say we've found at least 6 parallel line that are kinda the same length, We could check things a little
        # further just to be sure but that is not part of the MVP.
        if c >= 6:
            found = True
            viable.append(i)

    if found:
        verticalLines = []
        if len(viable) == 9:
            # All the lines were clear and present
            for i in range(len(viable)):
                verticalLines.append(vertical[viable[i]])
                l = verticalLines[i]
                '''
                print(str(i) + ": These are the 9 valid vertical: x0 = {} , y0 = {}, x1 = {}, y1 = {}"
                      .format(l[0],
                              l[1],
                              l[2],
                              l[3]))
                '''
                # cv.line(cdstP, (l[0], l[1]), (l[2], l[3]), (255, 0, 0), 3, cv.LINE_AA)
            diff = differenceCalculationOnLines(verticalLines)
            ## We are checking the median for the positions
            coordinates.append(verticalLines[0][0])  # The X0 Starting position (column)
            y1sorted = sorted(verticalLines, key=lambda x: x[3])
            print("Sorted based on Y1 : " + str(y1sorted))
            coordinates.append(y1sorted[4][3])
            # cv.circle(cdstP, (coordinates[0], coordinates[1]), 6, (0, 0, 255), 3)

            coordinates.append(verticalLines[8][0])  # The last column
            y0sorted = sorted(verticalLines, key=lambda x: x[1])
            print("Sorted based on Y0 : " + str(y0sorted))
            coordinates.append(y0sorted[4][1])
            # cv.circle(cdstP, (coordinates[2], coordinates[3]), 6, (0, 0, 255), 3)

            return coordinates


def FilterToCloseOnes(horizontal):
    filtered1 = []
    for i in range(0, len(horizontal)):
        l = horizontal[i]

        c = sum(map(lambda x: ((abs(x[0] - l[0]) < int(35*1/GLOBAL_COUNTER))
                               and (abs(x[2] - l[2]) < int(35*1/GLOBAL_COUNTER))  # How many with kinda same rowlength
                               and (abs(l[1] - x[1]) > int(25*2/GLOBAL_COUNTER))  # and a little further from it
                               and (abs(l[3] - x[3]) > int(25*2/GLOBAL_COUNTER)))
                    , horizontal))

        if c > 7:
            filtered1.append(horizontal[i])
            print("Coordinates:   sor1:{} , oszlop1: {} , sor2: {} , oszlop2: {} ".format(l[0], l[1], l[2], l[3]))
    return filtered1


def recursive(horizontal):
    filtered1 = FilterToCloseOnes(horizontal)
    global GLOBAL_COUNTER
    if len(filtered1) == 9 or GLOBAL_COUNTER == 5:
        GLOBAL_COUNTER = 1
        return filtered1
    else:
        GLOBAL_COUNTER += 1
        return recursive(filtered1)


# This function returns the supposed upper left and bottom-right points of the chess based on vertical lines
# In the following format [Px0,Py0,Px1,Py1]
def checkHorizontalLines(horizontal, cdstP):
    coordinates = [0, 0, 0, 0]
    found = False
    counter = 0
    isDone = False
    horizontal.sort(key=lambda x: x[1])  ##Sort based on y0  (the rownumber)
    print("Thesea are the horizontal lines: " + str(horizontal))
    filtered = recursive(horizontal)

    #############################################################################################################################################################################################################
    #############################################################################################################################################################################################################
    #############################################################################################################################################################################################################
    #############################################################################################################################################################################################################
    '''
    for i in range(0, len(horizontal)):
        l = horizontal[i]
        # -1 Because it would count itself in as well

        c = sum(map(lambda x: ((abs(x[0] - l[0]) < 30)
                               and (abs(x[2] - l[2]) < 30)  # How many with kinda same rowlength
                               and (abs(l[1] - x[1]) > 20)  # and a little further from it
                               and (abs(l[3] - x[3]) > 20))
                    , horizontal))

        if c > 7:
            found = True
            filtered1.append(horizontal[i])

    if found:
        possibly = []
        print("Viable list after the first search: elementNum({})".format(str(len(filtered1))) + str(filtered1))
        for i in range(0, len(filtered1)):
            l = filtered1[i]
            c = sum(map(lambda x: ((abs(x[0] - l[0]) < 20)
                                   and (abs(x[2] - l[2]) < 20)  # How many with kinda same rowlength
                                   and (abs(l[1] - x[1]) > 30)  # How many that is a little further from it
                                   and (abs(l[3] - x[3]) > 30))
                        , filtered1))
            if c == 8:
                filtered2.append(filtered1[i])

        for i in range(0, len(filtered2)):
            l = filtered2[i]
            c = sum(map(lambda x: ((abs(x[0] - l[0]) < 20)
                                   and (abs(x[2] - l[2]) < 20)  # How many with kinda same rowlength
                                   and (abs(l[1] - x[1]) > 30)  # How many that is a little further from it
                                   and (abs(l[3] - x[3]) > 30))
                        , filtered2))
            if c == 8:
                possibly.append(i)

        horizontalLines = []
        '''

    horizontalLines = []
    if len(filtered) == 9:
        # All the lines were clear and present
        for i in range(len(filtered)):
            horizontalLines.append(filtered[i])
            l = horizontalLines[i]
            '''
            print(str(i) + ": These are the 9 valid horizontalLines: x0 = {} , y0 = {}, x1 = {}, y1 = {}"
                  .format(l[0],
                          l[1],
                          l[2],
                          l[3]))
                          '''
            cv.line(cdstP, (l[0], l[1]), (l[2], l[3]), (255, 0, 255), 3, cv.LINE_AA)
        diff = differenceCalculationOnLines(horizontalLines, False)

        coordinates[3] = horizontalLines[8][3]
        coordinates[1] = horizontalLines[0][1]
        ## We are checking the median for the positions
        x0sorted = sorted(horizontalLines, key=lambda x: x[0])
        coordinates[0] = x0sorted[4][0]  # The X0 Starting position (column)

        cv.circle(cdstP, (coordinates[0], coordinates[1]), 3, (255, 0, 0), 3)

        x1sorted = sorted(horizontalLines, key=lambda x: x[2])
        coordinates[2] = x1sorted[4][2]

        cv.circle(cdstP, (coordinates[2], coordinates[3]), 3, (255, 0, 0), 3)
        # cv.imshow("Points",cdstP)
        cv.waitKey()
        return coordinates
    else:
        for i in range(len(filtered)):
            horizontalLines.append(filtered[i])
            l = horizontalLines[i]
            '''
            print("These are the horizontalLines: x0 = {} , y0 = {}, x1 = {}, y1 = {}"
                  .format(l[0],
                          l[1],
                          l[2],
                          l[3]))
            '''


# This gives back the coordinates for the given pictures Chessboard
def print_hi(pathOfTheFile):
    img = cv.imread(pathOfTheFile)
    # img = cv.imread('C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\chessdefault.png')
    edges = numpy.array([])

    dst = cv.Canny(img, 50, 200, edges, 3)
    cdstP = cv.cvtColor(dst, cv.COLOR_GRAY2BGR)

    # linesP = cv.HoughLinesP(dst, 1, np.pi / 180, 50, None, 50, 10)
    linesP = cv.HoughLinesP(dst, rho=1, theta=1 * np.pi / 180, threshold=50, minLineLength=500, maxLineGap=5)
    horizontal = []
    vertical = []
    height = np.size(img, 0)
    width = np.size(img, 1)
    if linesP is not None:

        #vertical.append([0, width, 0, 0])  # |---
        #vertical.append([width, height, width, 0])  # ---|
        #horizontal.append([0, 0, width, 0]) #_
        #horizontal.append([0, height, width, height])  #-
        for i in range(0, len(linesP)):
            l = linesP[i][0]
            if l[2] - l[0] < 5:
                vertical.append(l)
                cv.line(cdstP, (l[0], l[1]), (l[2], l[3]), (0, 0, 255), 3, cv.LINE_AA)  # This is for presentation only
            elif l[3] - l[1] < 5:
                horizontal.append(l)
                cv.line(cdstP, (l[0], l[1]), (l[2], l[3]), (0, 0, 255), 3, cv.LINE_AA)  # This is for presentation only


    #cv.imshow("lines", cdstP)
    #cv.waitKey()


    # We start with the vertical lines as we expect less of those in the form of false lines.
    pointsBasedOnVertical = []
    pointsBasedOnHorizontal = []

    pointsBasedOnVertical = checkVerticalLines(vertical, cdstP)
    print("Points based on the Vertical Line: x0: {}; y0: {}; x1: {}; y1: {}".format(pointsBasedOnVertical[0],
                                                                                     pointsBasedOnVertical[1],
                                                                                     pointsBasedOnVertical[2],
                                                                                     pointsBasedOnVertical[3]))

    pointsBasedOnHorizontal = checkHorizontalLines(horizontal, cdstP)

    print("Points based on the Horizontal Line: x0: {}; y0: {}; x1: {}; y1: {}".format(pointsBasedOnHorizontal[0],
                                                                                       pointsBasedOnHorizontal[1],
                                                                                       pointsBasedOnHorizontal[2],
                                                                                       pointsBasedOnHorizontal[3]))


    pointsMostLikely = (int((pointsBasedOnVertical[0] + pointsBasedOnHorizontal[0]) / 2 // 1),
                        int((pointsBasedOnVertical[1] + pointsBasedOnHorizontal[1]) / 2 // 1),
                        int((pointsBasedOnVertical[2] + pointsBasedOnHorizontal[2]) / 2 // 1),
                        int((pointsBasedOnVertical[3] + pointsBasedOnHorizontal[3]) / 2 // 1))
    print("\n Most likely:  " + str(pointsMostLikely))

    return pointsMostLikely

def getDifferencePicture(coordinates1, coordinates2, defaultFirst, defaultSecond):
    croppedImg = cv.cvtColor(
        cv.resize(cv.imread(defaultFirst)[coordinates1[1]:coordinates1[3], coordinates1[0]:coordinates1[2]],
                  (320, 320)), cv.COLOR_BGR2LAB)

    croppedImg2 = cv.cvtColor(
        cv.resize(cv.imread(defaultSecond)[coordinates2[1]:coordinates2[3], coordinates2[0]:coordinates2[2]],
                  (320, 320)), cv.COLOR_BGR2LAB)

    diff1 = cv.subtract(croppedImg2, croppedImg)
    diff2 =  cv.subtract(croppedImg, croppedImg2)
    diff = cv.add(diff1,diff2)
    return diff


def main():
    # We have a first and a second picture as a parameter
    f = open("C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\output.txt", "w")
    f.write("ERROR")
    f.close()
    defaultFirst = 'C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\FullPic\\try7.png'
    defaultSecond = 'C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\FullPic\\try8.png'
    if (len(sys.argv) > 2):
        print(sys.argv[1] + "\n" + sys.argv[2])
        defaultFirst = sys.argv[1]
        defaultSecond = sys.argv[2]
    coordinates1 = print_hi(defaultFirst)

    coordinates2 = print_hi(defaultSecond)

    diff = getDifferencePicture(coordinates1, coordinates2, defaultFirst, defaultSecond)
    kernel = np.ones((5, 5), np.float32) / 25
    diff = cv.filter2D(diff, -1, kernel)
    cv.imshow("difference", diff)
    cv.waitKey()

    diff = cv.cvtColor(diff, cv.COLOR_BGR2GRAY)
    ret,diff =  cv.threshold(diff,10,255,cv.THRESH_BINARY)

    #cv.imshow("difference",diff)
    #cv.waitKey()

    affected =  []

    for i in range(0, 8):
        for j in range(0,8):
            #CHecking the 8x8 avg intensity
            smallCropped = diff[i*40:(i+1)*40,j*40:(j+1)*40]
            intensity = 0
            for x in range(0,40):
                for y in range(0,40):
                    intensity += 255 if smallCropped[x,y] > 100  else smallCropped[x,y]
            avg = int(intensity/1600)
            print("({}|{})Difference intensity avg {}   on  {}   {}".format(i,j,avg,i*40,j*40))
            if avg > 30:
                affected.append((7-i, j))

    f = open("C:\\Users\\Kolozsi\\Documents\\MSc\\Kepfeldolgozas\\beadando\\output.txt", "w")
    for i in range(0,len(affected)):
        print(affected[i])
        f.write(str(affected[i])+";")

    f.close()



if __name__ == "__main__":
    main()
