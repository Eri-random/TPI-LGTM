import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { PopupIdeaComponent } from './popup-idea.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('PopupIdeaComponent', () => {
  let componente: PopupIdeaComponent;
  let fixture: ComponentFixture<PopupIdeaComponent>;
  let matDialogRefMock: any;

  beforeEach(async () => {
    matDialogRefMock = {
      close: jasmine.createSpy('close')
    };

    await TestBed.configureTestingModule({
      declarations: [PopupIdeaComponent],
      imports: [MatDialogModule, MatIconModule],
      providers: [
        { provide: MatDialogRef, useValue: matDialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PopupIdeaComponent);
    componente = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente', () => {
    expect(componente).toBeTruthy();
  });

  it('debería cerrar el diálogo cuando se llama a onNoClick', () => {
    componente.onNoClick();
    expect(matDialogRefMock.close).toHaveBeenCalled();
  });

  it('debería cerrar el diálogo con "generateWithCurrentData" cuando se llama a onGenerateWithCurrentData', () => {
    componente.onGenerateWithCurrentData();
    expect(matDialogRefMock.close).toHaveBeenCalledWith('generateWithCurrentData');
  });

  it('debería cerrar el diálogo con "loadNewData" cuando se llama a onLoadNewData', () => {
    componente.onLoadNewData();
    expect(matDialogRefMock.close).toHaveBeenCalledWith('loadNewData');
  });
});
